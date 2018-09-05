using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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

        ObservableCollection<Argument> Arguments { get; }

        IEnumerable<PortAnchor> Anchors { get; }

        IEditablePage ParentPage { get; }

        void SyncArguments();

        NodeExecutionResult Execute(NodeExecutionContext context);

        void Reset();
    }
}
