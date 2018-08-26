using System.Windows;

namespace Roro.Workflow.Wpf
{
    public class PortAnchorLink
    {
        public IEditablePort SourcePort { get; }

        public IEditableNode TargetNode { get; }

        public PortAnchor SourcePortAnchor { get; }

        public PortAnchor TargetNodeAnchor { get; }

        public Point StartPoint => GetPoint(this.SourcePort.ParentNode.Rect, SourcePortAnchor);

        public Point EndPoint => GetPoint(TargetNode.Rect, TargetNodeAnchor);

        public string Path { get; set; }

        public PortAnchorLink(IEditablePort sourcePort, PortAnchor sourcePortAnchor, IEditableNode targetNode, PortAnchor targetNodeAnchor)
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
