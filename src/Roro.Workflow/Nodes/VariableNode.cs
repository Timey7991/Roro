using System;
using System.Collections.Generic;

namespace Roro.Workflow
{
    public sealed class VariableNode : Node
    {
        public override bool CanEndLink => false;

        public override IEnumerable<PortAnchor> Anchors => new PortAnchor[] { };

        public override NodeExecutionResult Execute(NodeExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
