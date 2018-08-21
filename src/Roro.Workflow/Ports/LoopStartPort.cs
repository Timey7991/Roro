namespace Roro.Workflow
{
    public sealed class LoopStartPort : Port
    {
        public override PortAnchor DefaultAnchor => PortAnchor.None;

        public override PortAnchor[] Anchors => new PortAnchor[] { };
    }
}
