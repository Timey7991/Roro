using System.IO;

namespace Roro.Activities.Storage
{
    public class AppendTextFile : IAction
    {
        public Input<string> Path { get; set; }

        public Input<string> Text { get; set; }

        public void Execute()
        {
            File.AppendAllText(this.Path.RuntimeValue, this.Text.RuntimeValue);
        }
    }
}
