namespace Roro.Workflow
{
    public enum FlowState
    {
        Idle,
        Running,
        Pausing,
        Paused,
        Stopping,
        Stopped,
        Failed,
        Completed
    }
}
