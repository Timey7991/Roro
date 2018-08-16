using System.IO;

namespace Roro.Activities.Storage
{
    public class DirectoryExists : IDecision
    {
        public Input<string> Path { get; set; }

        public bool Execute()
        {
            return Directory.Exists(this.Path.RuntimeValue);
        }
    }
}
