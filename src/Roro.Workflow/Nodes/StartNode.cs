using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Roro.Workflow
{
    public sealed class StartNode : Node
    {
        public override string Name
        {
            get => "Start";
            set => base.Name = this.Name;
        }

        public NextPort Next { get; set; } = new NextPort();

        public override ObservableCollection<Argument> Arguments { get; } = new ObservableCollection<Argument>();

        public override IEnumerable<PortAnchor> Anchors => new PortAnchor[] { };

        public override void Reset()
        {
            this.Arguments.ToList().ForEach(x => x.RuntimeValue = null);
        }

        public override NodeExecutionResult Execute(NodeExecutionContext context)
        {
            return new NodeExecutionResult(this.ParentPage, this.Next.To);

            throw new NotImplementedException();
        }
    }
}
