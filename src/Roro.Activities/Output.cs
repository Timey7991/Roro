namespace Roro.Activities
{
    /// <typeparam name="T">
    /// Should be
    /// <see cref="string"/>,
    /// <see cref="decimal"/>,
    /// <see cref="bool"/>,
    /// <see cref="DateTime"/> or
    /// <see cref="DataTable"/>
    /// </typeparam>
    public interface Output<T>
    {
        string Name { get; }

        T RuntimeValue { set; }
    }
}
