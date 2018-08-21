using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Roro.Workflow
{
    public sealed class DecisionNode : Node
    {
        public TruePort True { get; set; } = new TruePort();

        public FalsePort False { get; set; } = new FalsePort();

        public XmlTypeHelper DecisionType { get; set; }

        public List<Argument> Arguments { get; set; } = new List<Argument>();

        public override PortAnchor[] Anchors => new PortAnchor[] { PortAnchor.Left, PortAnchor.Top };

        public override NodeExecutionResult Execute(NodeExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
