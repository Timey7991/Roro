using System.Collections.Generic;

namespace Roro.Workflow
{
    public sealed class FalsePort : Port
    {
        public override string Text => "No";

        public override PortAnchor DefaultAnchor => PortAnchor.Right;

        public override IEnumerable<PortAnchor> Anchors => new PortAnchor[] { PortAnchor.Right }; 
    }
}