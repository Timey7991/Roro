using System;
using System.Collections.Generic;

namespace Roro.Workflow
{
    public interface IEditableNode
    {
        Guid Id { get; }

        string Name { get; }

        NodeRect Rect { get; }

        void SetLocation(int x, int y);

        bool Selected { get; set; }

        bool CanEndLink { get; }

        IEnumerable<IEditablePort> Ports { get; } 

        IEnumerable<PortAnchor> Anchors { get; }

        IEditablePage ParentPage { get; }
    }
}
