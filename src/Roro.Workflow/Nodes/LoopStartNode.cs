using System;
using System.Collections.Generic;

namespace Roro.Workflow
{
    public sealed class LoopStartNode : Node
    {
        public NextPort Next { get; set; } = new NextPort();

        public LoopEndPort LoopEnd { get; set; } = new LoopEndPort();

        private bool _loopEnded;
        public bool LoopEnded
        {
            get => this._loopEnded;
            set => this.OnPropertyChanged(ref this._loopEnded, value);
        }

        public TypeWrapper LoopType { get; set; }

        public override IEnumerable<PortAnchor> Anchors => new PortAnchor[] { PortAnchor.Top };

        public override NodeExecutionResult Execute(NodeExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
