using System.IO;

namespace Roro.Activities.Storage
{
    public class CopyFile : IAction
    {
        public Input<string> Path { get; set; }

        public Input<string> DestinationPath { get; set; }

        public void Execute()
        {
            File.Copy(this.Path.RuntimeValue, this.DestinationPath.RuntimeValue, true);
        }
    }
}
