using System;
using System.Collections.Generic;

namespace Roro.Workflow
{
    public sealed class DecisionNode : Node
    {
        public TruePort True { get; set; } = new TruePort();

        public FalsePort False { get; set; } = new FalsePort();

        public TypeWrapper DecisionType { get; set; }

        public List<Argument> Arguments { get; set; } = new List<Argument>();

        public override IEnumerable<PortAnchor> Anchors => new PortAnchor[] { PortAnchor.Left, PortAnchor.Top };

        public override NodeExecutionResult Execute(NodeExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
