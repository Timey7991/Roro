using System;
using System.Collections.Generic;

namespace Roro.Workflow
{
    public interface IEditableFlow
    {
        Guid Id { get; }
        string Name { get; }

        IEnumerable<IEditablePage> Pages { get; }

        void AddPage(IEditablePage page);

        void RemovePage(IEditablePage page);
    }
}