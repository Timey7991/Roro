using System;
using System.Collections.Generic;

namespace Roro.Activities
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Property, AllowMultiple = false)]
    public class ValueOptionsAttribute : Attribute
    {
        public IEnumerable<object> Values { get; }

        public ValueOptionsAttribute(params object[] values)
        {
            this.Values = values;
        }
    }
}
