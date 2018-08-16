using System.IO;

namespace Roro.Activities.Storage
{
    public class DeleteDirectory : IAction
    {
        public Input<string> Path { get; set; }

        public void Execute()
        {
            Directory.Delete(this.Path.RuntimeValue, true);
        }
    }
}
