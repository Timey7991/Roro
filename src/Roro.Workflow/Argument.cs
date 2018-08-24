using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security;
using System.Xml.Serialization;

namespace Roro.Workflow
{
    public abstract class Argument
    {
        public string Name { get; set; }

        public abstract ArgumentDirection Direction { get; }

        public XmlTypeHelper ArgumentType { get; set; }

        public string Expression { get; set; }

        [XmlIgnore]
        public object DefaultValue { get; set; }

        [XmlIgnore]
        public object RuntimeValue { get; set; }

        public static IEnumerable<XmlTypeHelper> Types
        {
            get => new XmlTypeHelper[] {
                        new XmlTypeHelper(typeof(string)),
                        new XmlTypeHelper(typeof(decimal)),
                        new XmlTypeHelper(typeof(bool)),
                        new XmlTypeHelper(typeof(DateTime)),
                        new XmlTypeHelper(typeof(DataTable)),
                        new XmlTypeHelper(typeof(SecureString))
                    };
        }

        public static IEnumerable<ArgumentDirection> Directions
        {
            get => Enum.GetValues(typeof(ArgumentDirection)).Cast<ArgumentDirection>();
        }
    }
}