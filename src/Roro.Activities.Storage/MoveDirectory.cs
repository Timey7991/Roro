using System.IO;

namespace Roro.Activities.Storage
{
    public class MoveDirectory : IAction
    {
        public Input<string> Path { get; set; }

        public Input<string> DestinationPath { get; set; }

        public void Execute()
        {
            Directory.Move(this.Path.RuntimeValue, this.DestinationPath.RuntimeValue);
        }
    }
}
