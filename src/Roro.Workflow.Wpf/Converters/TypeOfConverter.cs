using System;
using System.Globalization;
using System.Windows.Data;

namespace Roro.Workflow.Wpf
{
    public class TypeOfConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = (value is TypeWrapper w ? w.WrappedType : value is Type t ? t : value?.GetType()) as Type;
            var baseType = (parameter is Type ? parameter : parameter?.GetType()) as Type;
            if (baseType is null)
            {
                return type;
            }
            else
            {
                return baseType.IsAssignableFrom(type) ? parameter : null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
