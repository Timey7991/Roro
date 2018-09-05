using Roro.Activities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Roro.Workflow
{
    public sealed class LoopStartNode : Node
    {
        private sealed class Loop : ILoop { public void Execute(object current) { } public IEnumerator GetEnumerator() => new object[0].GetEnumerator(); }

        public NextPort Next { get; set; } = new NextPort();

        public LoopEndPort LoopEnd { get; set; } = new LoopEndPort();

        private bool _loopEnded;
        public bool LoopEnded
        {
            get => this._loopEnded;
            set => this.OnPropertyChanged(ref this._loopEnded, value);
        }

        public TypeWrapper LoopType
        {
            get => this._loopType;
            set
            {
                if (typeof(ILoop).IsAssignableFrom(value.WrappedType))
                {
                    this.OnPropertyChanged(ref this._loopType, value);
                }
                else
                {
                    throw new InvalidCastException();
                }
            }
        }
        private TypeWrapper _loopType;

        private ILoop _loop;

        private IEnumerator _loopEnumerator;

        public override ObservableCollection<Argument> Arguments { get; } = new ObservableCollection<Argument>();

        public override IEnumerable<PortAnchor> Anchors => new PortAnchor[] { PortAnchor.Top };

        public override void Reset()
        {
            this.Arguments.ToList().ForEach(x => x.RuntimeValue = null);
            this._loop = default(ILoop);
            this._loopEnumerator = default(IEnumerator);
            this.LoopEnded = false;
        }

        public LoopStartNode() : this(new TypeWrapper(typeof(Loop)))
        {
            ;
        }

        public LoopStartNode(TypeWrapper type)
        {
            this.LoopType = type;
            this.CreateLoopTypeInstance();
            this.Name = type.Name;
        }

        public override NodeExecutionResult Execute(NodeExecutionContext context)
        {
            if (this.LoopEnded)
            {
                var loop = this._loop = this.CreateLoopTypeInstance();

                var inputs = loop.GetType().GetProperties()
                    .Where(prop => prop.PropertyType.GetGenericTypeDefinition() == typeof(Input<>)).ToList()
                    .Select(prop => prop.GetValue(loop)).Cast<Argument>().ToList();

                var variables = this.ParentPage.Nodes.Where(x => x is VariableNode).Cast<VariableNode>();

                // RuntimeValue = Expression (resolved using variables)
                inputs.ForEach(input => input.RuntimeValue = input.Expression);

                // assign input-arguments (once)
                this._loopEnumerator = loop.GetEnumerator();
                this.LoopEnded = false;
            }

            if (this._loopEnumerator.MoveNext())
            {
                var loop = this._loop;

                var outputs = loop.GetType().GetProperties()
                    .Where(prop => prop.PropertyType.GetGenericTypeDefinition() == typeof(Output<>)).ToList()
                    .Select(prop => prop.GetValue(loop)).Cast<Argument>().ToList();

                var variables = this.ParentPage.Nodes.Where(x => x is VariableNode).Cast<VariableNode>();

                loop.Execute(this._loopEnumerator.Current);

                // Expression (variable) = RuntimeValue
                outputs.ForEach(output => output.RuntimeValue = output.RuntimeValue); // TODO

                // assign output-arguments
                return new NodeExecutionResult(this.ParentPage, this.Next.To);
            }
            else
            {
                this.LoopEnded = true;
                return new NodeExecutionResult(this.ParentPage, this.LoopEnd.To);
            }
        }

        public override void SyncArguments() => this.CreateLoopTypeInstance();

        private ILoop CreateLoopTypeInstance()
        {
            var loop = (ILoop)Activator.CreateInstance(this.LoopType.WrappedType);

            var arguments = new List<Argument>();

            var inputs = loop.GetType().GetProperties()
                .Where(prop => prop.PropertyType.GetGenericTypeDefinition() == typeof(Input<>)).ToList();

            var outputs = loop.GetType().GetProperties()
                .Where(prop => prop.PropertyType.GetGenericTypeDefinition() == typeof(Output<>)).ToList();

            inputs.ForEach(prop =>
            {
                var genericArgs = prop.PropertyType.GetGenericArguments();
                var genericType = typeof(InArgument<>).MakeGenericType(genericArgs);
                var genericTypeInstance = Activator.CreateInstance(genericType) as InArgument;
                genericTypeInstance.Name = prop.Name;
                genericTypeInstance.ArgumentType = new TypeWrapper(genericArgs.First());
                prop.SetValue(loop, genericTypeInstance);
                arguments.Add(genericTypeInstance.ToNonGeneric());
            });

            outputs.ForEach(prop =>
            {
                var genericArgs = prop.PropertyType.GetGenericArguments();
                var genericType = typeof(OutArgument<>).MakeGenericType(genericArgs);
                var genericTypeInstance = Activator.CreateInstance(genericType) as OutArgument;
                genericTypeInstance.Name = prop.Name;
                genericTypeInstance.ArgumentType = new TypeWrapper(genericArgs.First());
                prop.SetValue(loop, genericTypeInstance);
                arguments.Add(genericTypeInstance.ToNonGeneric());
            });

            this.Arguments.ToList().ForEach(cachedArgument =>
            {
                if (arguments.FirstOrDefault(x => x.Name == cachedArgument.Name && x.Direction == cachedArgument.Direction) is Argument argument)
                {
                    argument.Expression = cachedArgument.Expression;
                }
            });

            this.Arguments.Clear();
            arguments.ForEach(x => this.Arguments.Add(x));

            return loop;
        }
    }
}
