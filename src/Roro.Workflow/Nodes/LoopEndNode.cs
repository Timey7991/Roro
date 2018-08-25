using System;
using System.Collections.Generic;

namespace Roro.Workflow
{
    public sealed class LoopEndNode : Node
    {
        public NextPort Next { get; set; } = new NextPort();

        public LoopStartPort LoopStart { get; set; } = new LoopStartPort();

        public override IEnumerable<PortAnchor> Anchors => new PortAnchor[] { PortAnchor.Top };

        public override NodeExecutionResult Execute(NodeExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
