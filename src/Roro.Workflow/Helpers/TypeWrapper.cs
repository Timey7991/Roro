using System;
using System.Xml;
using System.Xml.Serialization;

namespace Roro.Workflow
{
    /// <summary>
    /// A wrapper for <see cref="Type" /> to support serialization using <see cref="XmlSerializerHelper"/>.
    /// Please do not create implicit operators for <see cref="Type"/> from/to <see cref="TypeWrapper"/> conversion.
    /// </summary>
    public class TypeWrapper
    {
        [XmlIgnore]
        public Type WrappedType { get; private set; }

        public string Name => this.WrappedType.Name;

        [XmlText]
        public string FullName
        {
            get => this.WrappedType.FullName;
            set => this.WrappedType = Type.GetType(value) ?? Type.GetType(string.Format("{0}, {1}", value, value.Substring(0, value.LastIndexOf('.'))));
        }

        public string Namespace => this.WrappedType.Namespace;

        private TypeWrapper()
        {
            ;
        }

        public TypeWrapper(Type type)
        {
            this.WrappedType = type;
        }



        public override bool Equals(object obj)
        {
            return obj is TypeWrapper other ? this.WrappedType.Equals(other.WrappedType) : this.WrappedType.Equals(obj); 
        }

        public override int GetHashCode()
        {
            return this.WrappedType?.GetHashCode() ?? base.GetHashCode();
        }
    }
}
