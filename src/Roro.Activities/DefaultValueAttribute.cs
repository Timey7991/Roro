using System;

namespace Roro.Activities
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Property, AllowMultiple = false)]
    public class DefaultValueAttribute : Attribute
    {
        public object Value { get; }

        public DefaultValueAttribute(object value)
        {
            this.Value = value;
        }
    }
}
