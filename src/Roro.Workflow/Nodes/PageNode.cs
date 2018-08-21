using System;

namespace Roro.Workflow
{
    public sealed class PageNode : Node
    {
        public NextPort Next { get; set; } = new NextPort();

        private bool _pageEnded;
        public bool PageEnded
        {
            get => this._pageEnded;
            set => this.OnPropertyChanged(ref this._pageEnded, value);
        }

        private string _pageName;
        public string PageName
        {
            get => this._pageName ?? string.Empty;
            set => this.OnPropertyChanged(ref this._pageName, value);
        }

        public override PortAnchor[] Anchors => new PortAnchor[] { PortAnchor.Left, PortAnchor.Top };

        public override NodeExecutionResult Execute(NodeExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
