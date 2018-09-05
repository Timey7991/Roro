using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Roro.Workflow
{
    public sealed class PageNode : Node
    {
        public NextPort Next { get; set; } = new NextPort();

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

        public override void Reset()
        {
            this.Arguments.ToList().ForEach(x => x.RuntimeValue = null);
        }

        public override NodeExecutionResult Execute(NodeExecutionContext context)
        {
            if (this.ParentPage.ParentFlow.Pages.FirstOrDefault(x => x.Id == this.PageId || x.Name == this.PageName) is Page pageToCall)
            {
                this.PageId = pageToCall.Id;
                this.PageName = pageToCall.Name;

                pageToCall.Reset();
                pageToCall.CallbackNode = this;

                // set input-arguments for startNode

                return new NodeExecutionResult(pageToCall, pageToCall.StartNode.Id);
            }
            else if (this.PageId == Guid.Empty && string.IsNullOrEmpty(this.PageName))
            {
                return new NodeExecutionResult(this.ParentPage, this.Next.To);
            }
            else
            {
                throw new Exception("Page not found.");
            }
        }

        public override void SyncArguments()
        {
            if (this.ParentPage.ParentFlow.Pages.FirstOrDefault(x => x.Id == this.PageId) is Page page)
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
