using System;
using System.Data;
using System.IO;

namespace Roro.Activities.Storage
{
    public class GetFiles : IAction
    {
        public Input<string> Path { get; set; }

        [DefaultValue("*")]
        public Input<string> Pattern { get; set; }

        public Output<DataTable> Files { get; set; }

        public Output<decimal> Count { get; set; }

        public void Execute()
        {
            ExecuteInternal(this.Path.RuntimeValue, this.Pattern.RuntimeValue, out DataTable files);
            this.Files.RuntimeValue = files;
            this.Count.RuntimeValue = files.Rows.Count;
        }

        internal static void ExecuteInternal(string path, string pattern, out DataTable files)
        {
            files = new DataTable();
            files.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("Name", typeof(string)),
                new DataColumn("NameWithoutExtension", typeof(string)),
                new DataColumn("Extension", typeof(string)),
                new DataColumn("ParentDirectoryName", typeof(string)),
                new DataColumn("FullName", typeof(string)),
                new DataColumn("Length", typeof(decimal)),
                new DataColumn("IsReadOnly", typeof(bool)),
                new DataColumn("Attributes", typeof(string)),
                new DataColumn("CreationTime", typeof(DateTime)),
                new DataColumn("CreationTimeUtc", typeof(DateTime)),
                new DataColumn("LastAccessTime", typeof(DateTime)),
                new DataColumn("LastAccessTimeUtc", typeof(DateTime)),
                new DataColumn("LastWriteTime", typeof(DateTime)),
                new DataColumn("LastWriteTimeUtc", typeof(DateTime)),
            });

            var fileNames = Directory.GetFiles(path, pattern);
            foreach (var fileName in fileNames)
            {
                var fileInfo = new FileInfo(fileName);
                files.Rows.Add(
                    fileInfo.Name,
                    System.IO.Path.GetFileNameWithoutExtension(fileInfo.Name), // NameWithoutExtension
                    fileInfo.Extension,
                    fileInfo.DirectoryName, // ParentDirectoryName
                    fileInfo.FullName,
                    fileInfo.Length,
                    fileInfo.IsReadOnly,
                    fileInfo.Attributes.ToString(),
                    fileInfo.CreationTime,
                    fileInfo.CreationTimeUtc,
                    fileInfo.LastAccessTime,
                    fileInfo.LastAccessTimeUtc,
                    fileInfo.LastWriteTime,
                    fileInfo.LastWriteTimeUtc);
            }
        }
    }
}
