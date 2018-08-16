using System;

namespace Roro.Workflow.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var page = new XPage();
            for (var n = 0; n < 5; n++)
            {
                var node = new XNode();
                for (var p = 0; p < 3; p++)
                {
                    node.Ports.Add(new XPort());
                }
                page.XNodes.Add(node);
            }

            var xml = XmlSerializerHelper.ToString(page);
            Console.WriteLine();
            Console.WriteLine("SERIALIZE");
            Console.WriteLine();
            Console.WriteLine(xml);
            Console.WriteLine();
            Console.WriteLine("DESERIALIZE");
            Console.WriteLine();
            XmlSerializerHelper.ToObject<XPage>(xml);
        }
    }

    public class XPage
    {
        public NotifyCollectionHelper<XNode> XNodes { get; }

        public XPage()
        {
            Console.WriteLine("XPage ctor");
            this.XNodes = new NotifyCollectionHelper<XNode>();
            this.XNodes.CollectionChanged += XNodes_CollectionChanged;       
        }

        private void XNodes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Console.WriteLine("XNodes_CollectionChanged");
        }
    }

    public class XNode
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public NotifyCollectionHelper<XPort> Ports { get; }

        public XNode()
        {
            Console.WriteLine("XNode ctor");
            this.Id = Guid.NewGuid();
            this.Name = "This is " + Id.ToString();
            this.Ports = new NotifyCollectionHelper<XPort>();
            this.Ports.CollectionChanged += Ports_CollectionChanged;
        }

        private void Ports_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Console.WriteLine("Ports_CollectionChanged");
        }
    }

    public class XPort
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public XPort()
        {
            Console.WriteLine("XPort ctor");
            this.Id = Guid.NewGuid();
            this.Name = "This is " + Id.ToString();
        }
    }
}
