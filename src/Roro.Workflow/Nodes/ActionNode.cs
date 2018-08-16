using System;
using System.Collections.Generic;

namespace Roro.Workflow
{
    public sealed class ActionNode : Node
    {
        public NextPort Next { get; set; } = new NextPort();

        public string ActionType { get; set; }

        public List<Argument> Arguments { get; set; } = new List<Argument>();

        public override NodeExecutionResult Execute(NodeExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
