using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml.Serialization;

namespace Roro.Workflow.Tests
{
    public class Program
    {
        static void Main(string[] args)
        {
            var f = new Flow(Guid.NewGuid().ToString());
            var x = Expression.Evaluate("1 + 2", f.MainPage);
            Console.WriteLine(x);
        }
    }
}
