using Roro.Activities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Roro.Workflow
{
    public sealed class ActionNode : Node
    {
        private sealed class Action : IAction { public void Execute() { } }

        public NextPort Next { get; set; } = new NextPort();

        public TypeWrapper ActionType
        {
            get => this._actionType;
            set
            {
                if (typeof(IAction).IsAssignableFrom(value.WrappedType))
                {
                    this.OnPropertyChanged(ref this._actionType, value);
                }
                else
                {
                    throw new InvalidCastException();
                }
            }
        }
        private TypeWrapper _actionType = new TypeWrapper(typeof(Action));

        public override ObservableCollection<Argument> Arguments { get; } = new ObservableCollection<Argument>();

        public override IEnumerable<PortAnchor> Anchors => new PortAnchor[] { PortAnchor.Left, PortAnchor.Top };

        public ActionNode() : this(new TypeWrapper(typeof(Action)))
        {
            ;
        }

        public ActionNode(TypeWrapper type)
        {
            this.ActionType = type;
            this.CreateActionTypeInstance();
            this.Name = type.Name;
        }

        public override NodeExecutionResult Execute(NodeExecutionContext context)
        {
            var action = this.CreateActionTypeInstance();

            var inputs = action.GetType().GetProperties()
                .Where(prop => prop.PropertyType.GetGenericTypeDefinition() == typeof(Input<>)).ToList()
                .Select(prop => prop.GetValue(action)).Cast<Argument>().ToList();

            var outputs = action.GetType().GetProperties()
                .Where(prop => prop.PropertyType.GetGenericTypeDefinition() == typeof(Output<>)).ToList()
                .Select(prop => prop.GetValue(action)).Cast<Argument>().ToList();

            var variables = this.ParentPage.Nodes.Where(x => x is VariableNode).Cast<VariableNode>();

            // RuntimeValue = Expression (resolved using variables)
            inputs.ForEach(input => input.RuntimeValue = input.Expression);

            action.Execute();

            // Expression (variable) = RuntimeValue
            outputs.ForEach(output => output.RuntimeValue = output.RuntimeValue); // TODO

            return new NodeExecutionResult(this.ParentPage, this.Next.To);
        }

        public override void SyncArguments() => this.CreateActionTypeInstance();

        private IAction CreateActionTypeInstance()
        {
            var action = (IAction)Activator.CreateInstance(this.ActionType.WrappedType);

            var arguments = new List<Argument>();

            var inputs = action.GetType().GetProperties()
                .Where(prop => prop.PropertyType.GetGenericTypeDefinition() == typeof(Input<>)).ToList();

            var outputs = action.GetType().GetProperties()
                .Where(prop => prop.PropertyType.GetGenericTypeDefinition() == typeof(Output<>)).ToList();

            inputs.ForEach(prop =>
            {
                var genericArgs = prop.PropertyType.GetGenericArguments();
                var genericType = typeof(InArgument<>).MakeGenericType(genericArgs);
                var genericTypeInstance = Activator.CreateInstance(genericType) as InArgument;
                genericTypeInstance.Name = prop.Name;
                genericTypeInstance.ArgumentType = new TypeWrapper(genericArgs.First());
                prop.SetValue(action, genericTypeInstance);
                arguments.Add(genericTypeInstance.ToNonGeneric());
            });

            outputs.ForEach(prop =>
            {
                var genericArgs = prop.PropertyType.GetGenericArguments();
                var genericType = typeof(OutArgument<>).MakeGenericType(genericArgs);
                var genericTypeInstance = Activator.CreateInstance(genericType) as OutArgument;
                genericTypeInstance.Name = prop.Name;
                genericTypeInstance.ArgumentType = new TypeWrapper(genericArgs.First());
                prop.SetValue(action, genericTypeInstance);
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

            return action;
        }
    }
}