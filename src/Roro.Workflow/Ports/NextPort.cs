namespace Roro.Workflow
{
    public sealed class NextPort : Port
    {
        public override PortAnchor DefaultAnchor => PortAnchor.Bottom;

        public override PortAnchor[] Anchors => new PortAnchor[] { PortAnchor.Right, PortAnchor.Bottom };
    }
}