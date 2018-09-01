using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Roro.Workflow
{
    public sealed class EndNode : Node
    {
        public override string Name
        {
            get => "End";
            set => base.Name = this.Name;
        }

        public override ObservableCollection<Argument> Arguments { get; } = new ObservableCollection<Argument>();

        public override IEnumerable<PortAnchor> Anchors => new PortAnchor[] { PortAnchor.Left, PortAnchor.Top, PortAnchor.Right, PortAnchor.Bottom };

        public override NodeExecutionResult Execute(NodeExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
