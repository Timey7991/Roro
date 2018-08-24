using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Roro.Workflow
{
    public class XmlTypeHelper : IXmlSerializable
    {
        public Type SystemType { get; private set; }

        private XmlTypeHelper()
        {

        }

        public XmlTypeHelper(Type type)
        {
            this.SystemType = type;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            this.SystemType = Type.GetType(reader.ReadContentAsString());
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteString(this.SystemType.FullName);
        }

        public static implicit operator Type(XmlTypeHelper xmlType)
        {
            return xmlType.SystemType;
        }

        public static implicit operator XmlTypeHelper(Type type)
        {
            return new XmlTypeHelper(type);
        }

        public override bool Equals(object obj)
        {
            return obj is XmlTypeHelper other ? this.SystemType.Equals(other) : this.SystemType.Equals(obj); 
        }

        public override int GetHashCode()
        {
            return this.SystemType.GetHashCode();
        }
    }
}
