using System;
using System.Data;
using System.IO;

namespace Roro.Activities.Storage
{
    public class GetDirectories : IAction
    {
        public Input<string> Path { get; set; }

        [DefaultValue("*")]
        public Input<string> Pattern { get; set; }

        public Output<DataTable> Directories { get; set; }

        public Output<decimal> Count { get; set; }

        public void Execute()
        {
            ExecuteInternal(this.Path.RuntimeValue, this.Pattern.RuntimeValue, out DataTable directories);
            this.Directories.RuntimeValue = directories;
            this.Count.RuntimeValue = directories.Rows.Count;
        }

        internal static void ExecuteInternal(string path, string pattern, out DataTable directories)
        {
            directories = new DataTable();
            directories.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("Name", typeof(string)),
                new DataColumn("ParentDirectoryName", typeof(string)),
                new DataColumn("FullName", typeof(string)),
                new DataColumn("Attributes", typeof(string)),
                new DataColumn("CreationTime", typeof(DateTime)),
                new DataColumn("CreationTimeUtc", typeof(DateTime)),
                new DataColumn("LastAccessTime", typeof(DateTime)),
                new DataColumn("LastAccessTimeUtc", typeof(DateTime)),
                new DataColumn("LastWriteTime", typeof(DateTime)),
                new DataColumn("LastWriteTimeUtc", typeof(DateTime)),
            });

            var directoryNames = Directory.GetDirectories(path, pattern);
            foreach (var directoryName in directoryNames)
            {
                var directoryInfo = new DirectoryInfo(directoryName);
                directories.Rows.Add(
                    directoryInfo.Name,
                    directoryInfo.Parent.FullName, // ParentDirectoryName
                    directoryInfo.FullName,
                    directoryInfo.Attributes.ToString(),
                    directoryInfo.CreationTime,
                    directoryInfo.CreationTimeUtc,
                    directoryInfo.LastAccessTime,
                    directoryInfo.LastAccessTimeUtc,
                    directoryInfo.LastWriteTime,
                    directoryInfo.LastWriteTimeUtc);
            }
        }
    }
}
