using System.IO;

namespace Roro.Activities.Storage
{
    public class MoveFile : IAction
    {
        public Input<string> Path { get; set; }

        public Input<string> DestinationPath { get; set; }

        public void Execute()
        {
            File.Move(this.Path.RuntimeValue, this.DestinationPath.RuntimeValue);
        }
    }
}
