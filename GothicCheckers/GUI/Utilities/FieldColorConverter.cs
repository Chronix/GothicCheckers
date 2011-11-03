using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace GothicCheckers.GUI.Utilities
{
    [ValueConversion(typeof(FieldColor), typeof(SolidColorBrush))]
    public class FieldColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            FieldColor color = (FieldColor)value;

            if (color == FieldColor.Light)
            {
                return new SolidColorBrush(Colors.LightGray);
            }
            else
            {
                return new SolidColorBrush(Colors.Gray);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }
}
