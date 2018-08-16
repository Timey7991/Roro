using System.IO;

namespace Roro.Activities.Storage
{
    public class DeleteFile : IAction
    {
        public Input<string> Path { get; set; }

        public void Execute()
        {
            File.Delete(this.Path.RuntimeValue);
        }
    }
}
