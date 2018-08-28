using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Roro.Workflow
{
    public sealed class PreparationNode : Node
    {
        public NextPort Next { get; set; } = new NextPort();

        public override ObservableCollection<Argument> Arguments { get; } = new ObservableCollection<Argument>();

        public override IEnumerable<PortAnchor> Anchors => new PortAnchor[] { PortAnchor.Left, PortAnchor.Top };

        public override NodeExecutionResult Execute(NodeExecutionContext context)
        {
            throw new NotImplementedException();
        }

        public PreparationNode()
        {
            this.Name = "Assign";
        }
    }
}
