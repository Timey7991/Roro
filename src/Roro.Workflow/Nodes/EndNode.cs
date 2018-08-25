using System;
using System.Collections.Generic;

namespace Roro.Workflow
{
    public sealed class EndNode : Node
    {
        public override string Name => "End";

        public override IEnumerable<PortAnchor> Anchors => new PortAnchor[] { PortAnchor.Left, PortAnchor.Top, PortAnchor.Right, PortAnchor.Bottom };

        public override NodeExecutionResult Execute(NodeExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
