using System;

namespace Roro.Workflow
{
    public class NodeExecutionResult
    {
        public IEditablePage NextPage { get; }

        public Guid NextNodeId { get; }

        public NodeExecutionResult(IEditablePage nextPage, Guid nextNodeId)
        {
            this.NextPage = nextPage;
            this.NextNodeId = nextNodeId;
        }
    }
}