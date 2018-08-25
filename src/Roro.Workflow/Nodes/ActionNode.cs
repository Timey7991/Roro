using Roro.Activities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Roro.Workflow
{
    public sealed class ActionNode : Node
    {
        public NextPort Next { get; set; } = new NextPort();

        public TypeWrapper ActionType
        {
            get => this._actionType;
            set => this.OnPropertyChanged(ref this._actionType, value);
        }
        private TypeWrapper _actionType;

        public ObservableCollection<Argument> Arguments { get; } = new ObservableCollection<Argument>();

        public override IEnumerable<PortAnchor> Anchors => new PortAnchor[] { PortAnchor.Left, PortAnchor.Top };

        public override NodeExecutionResult Execute(NodeExecutionContext context)
        {
            throw new NotImplementedException();
        }

        public ActionNode()
        {

        }

        public ActionNode(TypeWrapper type)
        {
            if (typeof(IAction).IsAssignableFrom(type.WrappedType))
            {
                this.ActionType = type;
                this.Name = type.Name;
            }
            else
            {
                throw new ArgumentException("The type should implement IAction interface.", "type");
            }
        }

        public void SyncArguments()
        {
            if (this.ActionType is TypeWrapper && Activator.CreateInstance(this.ActionType.WrappedType) is IAction action)
            {
                var currentArguments = new List<Argument>();

                action.GetType().GetProperties()
                    .Where(prop => prop.PropertyType.GetGenericTypeDefinition() == typeof(Input<>)).ToList()
                    .ForEach(prop => {
                        var genericArgs = prop.PropertyType.GetGenericArguments();
                        var genericType = typeof(InArgument<>).MakeGenericType(genericArgs);
                        var genericTypeInstance = Activator.CreateInstance(genericType) as InArgument;
                        genericTypeInstance.Name = prop.Name;
                        genericTypeInstance.ArgumentType = new TypeWrapper(genericArgs.First());
                        prop.SetValue(action, genericTypeInstance);
                        currentArguments.Add(genericTypeInstance.ToNonGeneric());
                    });

                action.GetType().GetProperties()
                    .Where(prop => prop.PropertyType.GetGenericTypeDefinition() == typeof(Output<>)).ToList()
                    .ForEach(prop => {
                        var genericArgs = prop.PropertyType.GetGenericArguments();
                        var genericType = typeof(OutArgument<>).MakeGenericType(genericArgs);
                        var genericTypeInstance = Activator.CreateInstance(genericType) as OutArgument;
                        genericTypeInstance.Name = prop.Name;
                        genericTypeInstance.ArgumentType = new TypeWrapper(genericArgs.First());
                        prop.SetValue(action, genericTypeInstance);
                        currentArguments.Add(genericTypeInstance.ToNonGeneric());
                    });


                var cachedArguments = this.Arguments;

                cachedArguments.ToList().ForEach(cachedArgument =>
                {
                    if (currentArguments.FirstOrDefault(x => x.Name == cachedArgument.Name && x.Direction == cachedArgument.Direction) is Argument currentArgument)
                    {
                        currentArgument.Expression = cachedArgument.Expression;
                    }
                });

                this.Arguments.Clear();
                currentArguments.ToList().ForEach(x => this.Arguments.Add(x));
            }
            else
            {
                this.Arguments.Clear();
            }
        }
    }
}
