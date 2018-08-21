using System;

namespace Roro.Workflow
{
    public sealed class StartNode : Node
    {
        public override bool CanEndLink => false;

        public NextPort Next { get; set; } = new NextPort();

        public override PortAnchor[] Anchors => new PortAnchor[] { };

        public override NodeExecutionResult Execute(NodeExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
