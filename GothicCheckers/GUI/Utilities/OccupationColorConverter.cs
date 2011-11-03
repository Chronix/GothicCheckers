using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace GothicCheckers.GUI.Utilities
{
    [ValueConversion(typeof(PlayerColor), typeof(SolidColorBrush))]
    public class OccupationColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            PlayerColor color = (PlayerColor)value;

            switch (color)
            {
                case PlayerColor.None:
                    return Binding.DoNothing;
                case PlayerColor.Black:
                    return Colors.Black;
                case PlayerColor.White:
                    return Colors.White;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }
}
