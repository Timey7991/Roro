using System;
using System.Globalization;
using System.Windows.Data;

namespace Roro.Workflow.Wpf
{
    public class TypeOfConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is TypeWrapper w ? w.WrappedType : value is Type t ? t : value?.GetType()) as Type;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
