namespace Roro.Workflow
{
    public sealed class LoopEndPort : Port
    {
        public override PortAnchor DefaultAnchor => PortAnchor.None;

        public override PortAnchor[] Anchors => new PortAnchor[] { };
    }
}
