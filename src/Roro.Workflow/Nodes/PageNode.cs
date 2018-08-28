using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Roro.Workflow
{
    public sealed class PageNode : Node
    {
        public NextPort Next { get; set; } = new NextPort();

        public bool PageEnded
        {
            get => this._pageEnded;
            set => this.OnPropertyChanged(ref this._pageEnded, value);
        }
        private bool _pageEnded;

        public Guid PageId
        {
            get => this._pageId;
            set => this.OnPropertyChanged(ref this._pageId, value);
        }
        private Guid _pageId;

        public string PageName
        {
            get => this._pageName;
            set => this.OnPropertyChanged(ref this._pageName, value);
        }
        private string _pageName;

        public override ObservableCollection<Argument> Arguments { get; } = new ObservableCollection<Argument>();

        public override IEnumerable<PortAnchor> Anchors => new PortAnchor[] { PortAnchor.Left, PortAnchor.Top };

        public override NodeExecutionResult Execute(NodeExecutionContext context)
        {
            throw new NotImplementedException();
        }

        public override void SyncArguments()
        {
            if (this.ParentPage.ParentFlow.Pages.FirstOrDefault(x => x.Name == this.PageName) is Page page)
            {
                var arguments = new List<Argument>();

                page.StartNode.Arguments.Where(x => x is InArgument).ToList()
                    .ForEach(x =>
                        arguments.Add(new InArgument()
                        {
                            Name = x.Name,
                            ArgumentType = x.ArgumentType
                        }));
                
                this.Arguments.ToList().ForEach(cachedArgument =>
                {
                    if (arguments.FirstOrDefault(x => x.Name == cachedArgument.Name && x.Direction == cachedArgument.Direction) is Argument argument)
                    {
                        argument.Expression = cachedArgument.Expression;
                    }
                });

                this.Arguments.Clear();
                arguments.ForEach(x => this.Arguments.Add(x));
            }
            else
            {
                this.Arguments.Clear();
            }
        }
    }
}
