using System.Collections.Generic;

namespace Roro.Workflow
{
    public sealed class LoopStartPort : Port
    {
        public override PortAnchor DefaultAnchor => PortAnchor.None;

        public override IEnumerable<PortAnchor> Anchors => new PortAnchor[] { };
    }
}
