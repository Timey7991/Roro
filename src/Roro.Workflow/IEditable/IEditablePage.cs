using System;
using System.Collections.Generic;

namespace Roro.Workflow
{
    public interface IEditablePage
    {
        Guid Id { get; }

        string Name { get; }

        IEnumerable<IEditableNode> Nodes { get; }

        IEnumerable<IEditableNode> SelectedNodes { get; }

        void CancelPendingChanges();

        void CommitPendingChanges();

        void Add(IEditableNode node);

        string Cut();

        string Copy();

        void Paste(string xmlNodes);

        void Delete();

        void Undo();

        void Redo();
    }
}