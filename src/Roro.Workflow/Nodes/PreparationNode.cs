using System;
using System.Collections.Generic;

namespace Roro.Workflow
{
    public sealed class PreparationNode : Node
    {
        public NextPort Next { get; set; } = new NextPort();

        public List<Argument> Arguments { get; set; } = new List<Argument>();

        public override IEnumerable<PortAnchor> Anchors => new PortAnchor[] { PortAnchor.Left, PortAnchor.Top };

        public override NodeExecutionResult Execute(NodeExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
