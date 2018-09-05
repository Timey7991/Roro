using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Roro.Workflow
{
    public sealed class EndNode : Node
    {
        public override string Name
        {
            get => "End";
            set => base.Name = this.Name;
        }

        public override ObservableCollection<Argument> Arguments { get; } = new ObservableCollection<Argument>();

        public override IEnumerable<PortAnchor> Anchors => new PortAnchor[] { PortAnchor.Left, PortAnchor.Top, PortAnchor.Right, PortAnchor.Bottom };

        public override void Reset()
        {
            this.Arguments.ToList().ForEach(x => x.RuntimeValue = null);
        }

        public override NodeExecutionResult Execute(NodeExecutionContext context)
        {
            var parentPage = this.ParentPage as Page;
            var callbackNode = parentPage.CallbackNode as PageNode;
            var callbackPage = callbackNode?.ParentPage;

            // set output-arguments for PageNode

            return new NodeExecutionResult(callbackPage, callbackNode?.Next.To ?? Guid.Empty);
        }
    }
}
