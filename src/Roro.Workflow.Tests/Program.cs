using System;

namespace Roro.Workflow.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var actionNode = new ActionNode();
            var xml = XmlSerializerHelper.ToString(actionNode);
            Console.WriteLine(xml);
            var obj = XmlSerializerHelper.ToObject<ActionNode>(xml);
            var xmlx = XmlSerializerHelper.ToString(obj);
            Console.WriteLine(xmlx);
        }
    }
}
