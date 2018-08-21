using System.Windows;

namespace Roro.Workflow.Wpf
{
    public class PortAnchorLink
    {
        public Port SourcePort { get; }

        public Node TargetNode { get; }

        public PortAnchor SourcePortAnchor { get; }

        public PortAnchor TargetNodeAnchor { get; }

        public Point StartPoint => GetPoint(this.SourcePort.ParentNode.Bounds, SourcePortAnchor);

        public Point EndPoint => GetPoint(TargetNode.Bounds, TargetNodeAnchor);

        public double VectorLength => (EndPoint - StartPoint).Length;

        public PortAnchorLink(Port sourcePort, PortAnchor sourcePortAnchor, Node targetNode, PortAnchor targetNodeAnchor)
        {
            this.SourcePort = sourcePort;
            this.SourcePortAnchor = sourcePortAnchor;
            this.TargetNode = targetNode;
            this.TargetNodeAnchor = targetNodeAnchor;
        }

        private Point GetPoint(NodeRect rect, PortAnchor anchor)
        {
            switch (anchor)
            {
                case PortAnchor.Left:
                    return new Point(rect.X, rect.Y + rect.Height / 2);
                case PortAnchor.Top:
                    return new Point(rect.X + rect.Width / 2, rect.Y);
                case PortAnchor.Right:
                    return new Point(rect.X + rect.Width, rect.Y + rect.Height / 2);
                case PortAnchor.Bottom:
                    return new Point(rect.X + rect.Width / 2, rect.Y + rect.Height);
                default:
                    return new Point();
            }
        }
    }

}
