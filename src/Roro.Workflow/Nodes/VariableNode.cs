using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Roro.Workflow
{
    public sealed class VariableNode : Node
    {
        public override ObservableCollection<Argument> Arguments => null;

        public override IEnumerable<PortAnchor> Anchors => new PortAnchor[] { };

        public override NodeExecutionResult Execute(NodeExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
