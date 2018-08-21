using System;
using System.Data;
using System.Security;

namespace Roro.Activities
{
    /// <typeparam name="T">
    /// Should be
    /// <see cref="String"/>,
    /// <see cref="Decimal"/>,
    /// <see cref="Boolean"/>,
    /// <see cref="DateTime"/>
    /// <see cref="DataTable"/> or
    /// <see cref="SecureString"/>
    /// </typeparam>
    public interface Output<T>
    {
        string Name { get; }

        T RuntimeValue { set; }
    }
}
