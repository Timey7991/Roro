using System;
using System.Collections.Generic;
using System.Data;
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
                        new TypeWrapper(typeof(DataTable))
                    };
        }

        public static object GetTypeDefaultValue(TypeWrapper type)
        {
            if (type.WrappedType == typeof(string))
            {
                return string.Empty;
            }
            else if (type.WrappedType == typeof(decimal))
            {
                return (decimal)0;
            }
            else if (type.WrappedType == typeof(bool))
            {
                return false;
            }
            else if (type.WrappedType == typeof(DateTime))
            {
                return DateTime.MinValue;
            }
            else if (type.WrappedType == typeof(DataTable))
            {
                return new DataTable();
            }
            //else if (type.WrappedType == typeof(SecureString))
            //{
            //    return new SecureString();
            //}
            else
            {
                throw new NotSupportedException();
            }
        }
    }
}