using System;
using System.Collections.Generic;

namespace Roro.Workflow
{
    public sealed class StartNode : Node
    {
        public override string Name => "Start";

        public override bool CanEndLink => false;

        public NextPort Next { get; set; } = new NextPort();

        public override IEnumerable<PortAnchor> Anchors => new PortAnchor[] { };

        public override NodeExecutionResult Execute(NodeExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
