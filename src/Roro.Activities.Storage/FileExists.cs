using System.IO;

namespace Roro.Activities.Storage
{
    public class FileExists : IDecision
    {
        public Input<string> Path { get; set; }

        public bool Execute()
        {
            return File.Exists(this.Path.RuntimeValue);
        }
    }
}
