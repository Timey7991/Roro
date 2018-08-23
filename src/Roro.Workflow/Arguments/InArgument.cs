using Roro.Activities;

namespace Roro.Workflow
{
    public class InArgument : Argument
    {
        public override ArgumentDirection Direction => ArgumentDirection.In;

        public InArgument ToNonGeneric()
        {
            return new InArgument()
            {
                Name = Name,
                ArgumentType = ArgumentType,
                Expression = Expression
            };
        }
    }

    public class InArgument<T> : InArgument, Input<T>
    {
        T Input<T>.RuntimeValue
        {
            get => (T)base.RuntimeValue;
        }
    }
}