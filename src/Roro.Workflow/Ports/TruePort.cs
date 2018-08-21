using System.Collections.Generic;

namespace Roro.Workflow
{
    public sealed class TruePort : Port
    {
        public override string Text => "Yes";

        public override PortAnchor DefaultAnchor => PortAnchor.Bottom;

        public override PortAnchor[] Anchors => new PortAnchor[] { PortAnchor.Bottom };
    }
}