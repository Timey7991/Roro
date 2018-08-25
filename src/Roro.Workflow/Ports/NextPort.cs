using System.Collections.Generic;

namespace Roro.Workflow
{
    public sealed class NextPort : Port
    {
        public override PortAnchor DefaultAnchor => PortAnchor.Bottom;

        public override IEnumerable<PortAnchor> Anchors => new PortAnchor[] { PortAnchor.Right, PortAnchor.Bottom };
    }
}