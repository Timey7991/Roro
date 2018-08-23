using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Roro.Workflow
{
    public class XmlTypeHelper : IXmlSerializable
    {
        private Type _type;

        private XmlTypeHelper()
        {

        }

        private XmlTypeHelper(Type type)
        {
            this._type = type;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            this._type = Type.GetType(reader.ReadContentAsString());
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteString(this._type.FullName);
        }

        public static implicit operator Type(XmlTypeHelper xmlType)
        {
            return xmlType._type;
        }

        public static implicit operator XmlTypeHelper(Type type)
        {
            return new XmlTypeHelper(type);
        }
    }
}
