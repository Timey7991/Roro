using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace Roro.Workflow.Wpf
{
    public class PropertyWrapper : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return null;
            }
            else if (parameter is string)
            {
                return new NotifyPropertyWrapper(value, parameter.ToString());
            }
            else
            {
                throw new ArgumentNullException("parameter");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public class NotifyPropertyWrapper : NotifyPropertyHelper
        {
            private readonly object _dataContext;

            private readonly PropertyInfo _path;

            public object Value
            {
                get => this._path.GetValue(this._dataContext);
                set
                {
                    this._path.SetValue(this._dataContext, value);
                    this.OnPropertyChanged(ref this._value, value);
                }
            }
            private object _value;

            public NotifyPropertyWrapper(object obj, string propertyName)
            {
                this._dataContext = obj;
                this._path = obj.GetType().GetProperty(propertyName);
            }
        }
    }
}
