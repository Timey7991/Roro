using System.Collections.Generic;

namespace Roro.Workflow
{
    public sealed class TruePort : Port
    {
        public override string Text => "Yes";

        public override PortAnchor DefaultAnchor => PortAnchor.Bottom;

        public override IEnumerable<PortAnchor> Anchors => new PortAnchor[] { PortAnchor.Bottom };
    }
}