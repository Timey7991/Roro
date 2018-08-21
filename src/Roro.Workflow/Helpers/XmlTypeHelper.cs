using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Roro.Workflow
{
    public class XmlTypeHelper : IXmlSerializable
    {
        private string _typeName;

        private XmlTypeHelper()
        {

        }

        private XmlTypeHelper(string typeName)
        {
            this._typeName = typeName;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            this._typeName = reader.ReadContentAsString();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteString(this._typeName);
        }

        public static implicit operator Type(XmlTypeHelper type)
        {
            return Type.GetType(type._typeName);
        }

        public static implicit operator XmlTypeHelper(Type type)
        {
            return new XmlTypeHelper(type.FullName);
        }
    }
}
