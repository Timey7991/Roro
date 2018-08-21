using System;

namespace Roro.Workflow
{
    public sealed class EndNode : Node
    {
        public override PortAnchor[] Anchors => new PortAnchor[] { PortAnchor.Left, PortAnchor.Top, PortAnchor.Right, PortAnchor.Bottom };

        public override NodeExecutionResult Execute(NodeExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
