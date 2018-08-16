using System;

namespace Roro.Workflow
{
    public sealed class LoopEndNode : Node
    {
        public NextPort Next { get; set; } = new NextPort();

        public LoopStartPort LoopStart { get; set; } = new LoopStartPort();

        public override NodeExecutionResult Execute(NodeExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
