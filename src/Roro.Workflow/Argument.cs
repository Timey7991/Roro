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

        public TypeWrapper ArgumentType { get; set; }

        public string Expression { get; set; }

        [XmlIgnore]
        public object DefaultValue { get; set; }

        [XmlIgnore]
        public object RuntimeValue { get; set; }

        public abstract Argument ToNonGeneric();

        public static IEnumerable<TypeWrapper> Types
        {
            get => new TypeWrapper[] {
                        new TypeWrapper(typeof(string)),
                        new TypeWrapper(typeof(decimal)),
                        new TypeWrapper(typeof(bool)),
                        new TypeWrapper(typeof(DateTime)),
                        new TypeWrapper(typeof(DataTable)),
                        new TypeWrapper(typeof(SecureString))
                    };
        }
    }
}