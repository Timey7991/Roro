using Roro.Activities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Roro.Workflow
{
    public sealed class ActionNode : Node
    {
        public NextPort Next { get; set; } = new NextPort();

        public XmlTypeHelper ActionType
        {
            get => this._actionType;
            set => this.OnPropertyChanged(ref this._actionType, value);
        }
        private XmlTypeHelper _actionType;

        public NotifyCollectionHelper<Argument> Arguments { get; } = new NotifyCollectionHelper<Argument>();

        public override PortAnchor[] Anchors => new PortAnchor[] { PortAnchor.Left, PortAnchor.Top };

        public override NodeExecutionResult Execute(NodeExecutionContext context)
        {
            throw new NotImplementedException();
        }

        public void SyncArguments()
        {
            if (Activator.CreateInstance(this.ActionType) is IAction action)
            {
                var currentArguments = new List<Argument>();

                action.GetType().GetProperties()
                    .Where(prop => prop.PropertyType.GetGenericTypeDefinition() == typeof(Input<>)).ToList()
                    .ForEach(prop => {
                        var genericArgs = prop.PropertyType.GetGenericArguments();
                        var genericType = typeof(InArgument<>).MakeGenericType(genericArgs);
                        var genericTypeInstance = Activator.CreateInstance(genericType) as InArgument;
                        genericTypeInstance.Name = prop.Name;
                        genericTypeInstance.ArgumentType = genericArgs.First();
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
                        genericTypeInstance.ArgumentType = genericArgs.First();
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
