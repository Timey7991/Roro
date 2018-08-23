using Roro.Activities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Roro.Workflow
{
    public sealed class ActionNode : Node
    {
        public NextPort Next { get; set; } = new NextPort();

        public XmlTypeHelper ActionType { get; set; }

        public List<Argument> Arguments { get; set; } = new List<Argument>();

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

                cachedArguments.ForEach(cachedArgument =>
                {
                    if (currentArguments.FirstOrDefault(x => x.Name == cachedArgument.Name) is Argument currentArgument)
                    {
                        currentArgument.Expression = cachedArgument.Expression;
                    }
                });

                this.Arguments = currentArguments;
            }
            else
            {
                this.Arguments.Clear();
            }
        }
    }
}
