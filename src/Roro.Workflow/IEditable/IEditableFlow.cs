using System;
using System.Collections.Generic;

namespace Roro.Workflow
{
    public interface IEditableFlow
    {
        #region DATA

        Guid Id { get; }

        string Name { get; }

        IEnumerable<IEditablePage> Pages { get; }

        IEditablePage SelectedPage { get; }

        void AddPage(IEditablePage page);

        void RemovePage(IEditablePage page);

        #endregion

        #region RUNNER

        FlowState State { get; }

        IEditablePage NextPage { get; }

        IEditableNode NextNode { get; }

        void Run();

        void Pause();

        void Stop();

        void Reset();

        Flow.Dispatcher Dispatch { get; set; }

        #endregion
    }
}