using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Roro.Workflow
{
    public abstract class NotifyPropertyHelper : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged<T>(ref T property, T value, [CallerMemberName] string name = default(string))
        {
            property = value;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
