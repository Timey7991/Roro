using System;
using System.Collections.Generic;
using System.Text;

namespace Roro.Workflow
{
    public class NodeLogItem
    {
        public string Page { get; set; }

        public string Node { get; set; }

        public string Activity { get; set; }

        public IEnumerable<Argument> Arguments { get; set; }

        public string NextPage { get; set; }

        public string NextNode { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public TimeSpan HandlingTime => this.EndTime - this.StartTime; 
    }
}
