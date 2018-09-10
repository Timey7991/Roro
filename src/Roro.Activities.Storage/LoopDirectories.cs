using System;
using System.Collections;
using System.Data;

namespace Roro.Activities.Storage
{
    public class LoopDirectories : ILoop
    {
        public Input<string> Path { get; set; }

        [DefaultValue("*")]
        public Input<string> Pattern { get; set; }

        public Output<string> Name { get; set; }

        public Output<string> ParentDirectoryName { get; set; }

        public Output<string> FullName { get; set; }

        public Output<string> Attributes { get; set; }

        public Output<DateTime> CreationTime { get; set; }

        public Output<DateTime> CreationTimeUtc { get; set; }

        public Output<DateTime> LastAccessTime { get; set; }

        public Output<DateTime> LastAccessTimeUtc { get; set; }

        public Output<DateTime> LastWriteTime { get; set; }

        public Output<DateTime> LastWriteTimeUtc { get; set; }

        public IEnumerator GetEnumerator()
        {
            ListDirectories.ExecuteInternal(this.Path.RuntimeValue, this.Pattern.RuntimeValue, out DataTable directories);
            return directories.Rows.GetEnumerator();
        }

        public void Execute(object current)
        {
            var dataRow = current as DataRow;
            SetOutput(this.Name, dataRow);
            SetOutput(this.ParentDirectoryName, dataRow);
            SetOutput(this.FullName, dataRow);
            SetOutput(this.Attributes, dataRow);
            SetOutput(this.CreationTime, dataRow);
            SetOutput(this.CreationTimeUtc, dataRow);
            SetOutput(this.LastAccessTime, dataRow);
            SetOutput(this.LastAccessTimeUtc, dataRow);
            SetOutput(this.LastWriteTime, dataRow);
            SetOutput(this.LastWriteTimeUtc, dataRow);
        }

        private void SetOutput<T>(Output<T> output, DataRow dataRow)
        {
            output.RuntimeValue = (T)dataRow[output.Name];
        }
    }
}
