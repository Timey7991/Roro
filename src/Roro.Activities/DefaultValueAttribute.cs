using System;

namespace Roro.Activities
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Property, AllowMultiple = false)]
    public class DefaultValueAttribute : Attribute
    {
        public DefaultValueAttribute(object defaultValue)
        {

        }
    }
}
