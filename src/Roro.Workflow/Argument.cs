using System;
using System.Xml.Serialization;

namespace Roro.Workflow
{
    public abstract class Argument
    {
        public string Name { get; set; }

        public abstract ArgumentDirection Direction { get; }

        public Type ArgumentType { get; set; }

        public string Expression { get; set; }

        [XmlIgnore]
        public object DefaultValue { get; set; }

        [XmlIgnore]
        public object RuntimeValue { get; set; }
    }
}