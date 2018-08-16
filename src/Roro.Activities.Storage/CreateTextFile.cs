using System.IO;

namespace Roro.Activities.Storage
{
    public class CreateTextFile : IAction
    {
        public Input<string> Path { get; set; }

        public void Execute()
        {
            File.CreateText(this.Path.RuntimeValue);
        }
    }
}
