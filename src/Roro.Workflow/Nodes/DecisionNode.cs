using Roro.Activities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Roro.Workflow
{
    public sealed class DecisionNode : Node
    {
        private sealed class Decision : IDecision { public bool Execute() => true; }

        public TruePort True { get; set; } = new TruePort();

        public FalsePort False { get; set; } = new FalsePort();

        public TypeWrapper DecisionType
        {
            get => this._decisionType;
            set
            {
                if (typeof(IDecision).IsAssignableFrom(value.WrappedType))
                {
                    this.OnPropertyChanged(ref this._decisionType, value);
                }
                else
                {
                    throw new InvalidCastException();
                }
            }
        }
        private TypeWrapper _decisionType;

        public override ObservableCollection<Argument> Arguments { get; } = new ObservableCollection<Argument>();

        public override IEnumerable<PortAnchor> Anchors => new PortAnchor[] { PortAnchor.Left, PortAnchor.Top };

        public DecisionNode() : this(new TypeWrapper(typeof(Decision)))
        {
            ;
        }

        public DecisionNode(TypeWrapper type)
        {
            this.DecisionType = type;
            this.Name = type.Name;
        }

        public override NodeExecutionResult Execute(NodeExecutionContext context)
        {
            var decision = this.CreateDecisionTypeInstance();

            var inputs = decision.GetType().GetProperties()
                .Where(prop => prop.PropertyType.GetGenericTypeDefinition() == typeof(Input<>)).ToList()
                .Select(prop => prop.GetValue(decision)).Cast<Argument>().ToList();

            var variables = this.ParentPage.Nodes.Where(x => x is VariableNode).Cast<VariableNode>();

            // RuntimeValue = Expression (resolved using variables)
            inputs.ForEach(input => input.RuntimeValue = input.Expression);

            var result = decision.Execute();

            return new NodeExecutionResult(this.ParentPage, result ? this.True.To : this.False.To);
        }

        public override void SyncArguments() => this.CreateDecisionTypeInstance();

        private IDecision CreateDecisionTypeInstance()
        {
            var decision = (IDecision)Activator.CreateInstance(this.DecisionType.WrappedType);

            var arguments = new List<Argument>();

            var inputs = decision.GetType().GetProperties()
                .Where(prop => prop.PropertyType.GetGenericTypeDefinition() == typeof(Input<>)).ToList();
            
            inputs.ForEach(prop =>
            {
                var genericArgs = prop.PropertyType.GetGenericArguments();
                var genericType = typeof(InArgument<>).MakeGenericType(genericArgs);
                var genericTypeInstance = Activator.CreateInstance(genericType) as InArgument;
                genericTypeInstance.Name = prop.Name;
                genericTypeInstance.ArgumentType = new TypeWrapper(genericArgs.First());
                prop.SetValue(decision, genericTypeInstance);
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

            return decision;
        }
    }
}
