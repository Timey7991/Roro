using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Roro.Workflow
{
    public sealed class LoopEndNode : Node
    {
        public NextPort Next { get; set; } = new NextPort();

        public LoopStartPort LoopStart { get; set; } = new LoopStartPort();

        public override ObservableCollection<Argument> Arguments => null;

        public override IEnumerable<PortAnchor> Anchors => new PortAnchor[] { PortAnchor.Top };

        public override NodeExecutionResult Execute(NodeExecutionContext context)
        {
            var loopStartNode = this.ParentPage.Nodes
                                    .Where(x => x.Id == this.LoopStart.To)
                                    .Cast<LoopStartNode>().First();
            if (loopStartNode.LoopEnded)
            {
                return new NodeExecutionResult(this.ParentPage, this.Next.To);
            }
            else
            {
                return new NodeExecutionResult(this.ParentPage, this.LoopStart.To);
            }
        }
    }
}