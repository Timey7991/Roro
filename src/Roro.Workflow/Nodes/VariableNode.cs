using System;

namespace Roro.Workflow
{
    public sealed class VariableNode : Node
    {
        public override bool CanEndLink => false;

        public override NodeExecutionResult Execute(NodeExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
