using System;
using System.Collections.Generic;

namespace Roro.Workflow
{
    public interface IEditableNode
    {
        Guid Id { get; }

        string Name { get; }

        bool Selected { get; set; }

        NodeRect Rect { get; }

        void SetLocation(int x, int y);

        IEnumerable<IEditablePort> Ports { get; } 

        IEnumerable<PortAnchor> Anchors { get; }

        IEditablePage ParentPage { get; }

        void SyncArguments();
    }
}
