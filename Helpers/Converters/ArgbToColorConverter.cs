using System;
using System.Globalization;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Controls;

namespace Kalendarzyk.Helpers.Converters
{
    public class ArgbToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is string))
                return Colors.Transparent;

            string argb = value as string;

            if (argb.StartsWith("#"))
                argb = argb.Substring(1);

            if (argb.Length != 8)
                return Colors.Transparent;

            return Color.FromArgb(argb);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
