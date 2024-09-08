using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Kalendarzyk.Helpers.Converters
{
    public class MultiValueBoolToColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2)
                return Colors.Black; // Default color if there are not exactly 2 inputs

            bool isSelected = values[0] is bool b && b;
            string colorString = values[1] as string;

            if (isSelected)
            {
                // Return a color based on the selection status
                return Colors.Red; // Change this to your desired color
            }
            else
            {
                // Convert the color string to a Color object
                if (Color.TryParse(colorString, out var color))
                {
                    return color;
                }
                return Colors.Black; // Default color if parsing fails
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
