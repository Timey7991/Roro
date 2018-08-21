using System;
using System.Collections.Generic;
using System.Data;
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

        public static IEnumerable<Type> GetArgumentTypes()
        {
            return new Type[]
            {
                typeof(string),
                typeof(decimal),
                typeof(bool),
                typeof(DateTime),
                typeof(DataTable),
                typeof(SecureString)
            };
        }
    }
}