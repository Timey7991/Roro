using System.IO;

namespace Roro.Activities.Storage
{
    public class CreateDirectory : IAction
    {
        public Input<string> Path { get; set; }

        public void Execute()
        {
            Directory.CreateDirectory(this.Path.RuntimeValue);
        }
    }
}
