using System;
using System.Collections.Generic;

namespace Roro.Workflow
{
    public interface IEditablePort
    {
        Guid To { get; }

        PortAnchor CurrentAnchor { get; set; }

        PortAnchor DefaultAnchor { get; }

        IEnumerable<PortAnchor> Anchors { get; }

        IEditableNode ParentNode { get; }

        void Connect(IEditableNode node);
    }
}