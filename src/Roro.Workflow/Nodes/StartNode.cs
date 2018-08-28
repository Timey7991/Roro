using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Roro.Workflow
{
    public sealed class StartNode : Node
    {
        public override string Name
        {
            get => "Start";
            set => base.Name = value;
        }

        public NextPort Next { get; set; } = new NextPort();

        public override ObservableCollection<Argument> Arguments { get; } = new ObservableCollection<Argument>();

        public override IEnumerable<PortAnchor> Anchors => new PortAnchor[] { };

        public override NodeExecutionResult Execute(NodeExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
