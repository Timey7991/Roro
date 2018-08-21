using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Roro.Workflow
{
    public sealed class ActionNode : Node
    {
        public NextPort Next { get; set; } = new NextPort();

        public XmlTypeHelper ActionType { get; set; }

        public List<Argument> Arguments { get; set; } = new List<Argument>();

        public override PortAnchor[] Anchors => new PortAnchor[] { PortAnchor.Left, PortAnchor.Top };

        public override NodeExecutionResult Execute(NodeExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
