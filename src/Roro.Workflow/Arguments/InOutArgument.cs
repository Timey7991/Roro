namespace Roro.Workflow
{
    public class InOutArgument : Argument
    {
        public override ArgumentDirection Direction => ArgumentDirection.InOut;

        public override Argument ToNonGeneric()
        {
            return new InOutArgument()
            {
                Name = Name,
                ArgumentType = ArgumentType,
                Expression = Expression
            };
        }
    }
}