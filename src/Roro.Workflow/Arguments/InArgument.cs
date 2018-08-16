using Roro.Activities;

namespace Roro.Workflow
{
    public class InArgument : Argument
    {
        public override ArgumentDirection Direction => ArgumentDirection.In;
    }

    public class InArgument<T> : InArgument, Input<T>
    {
        T Input<T>.RuntimeValue
        {
            get => (T)base.RuntimeValue;
        }
    }
}