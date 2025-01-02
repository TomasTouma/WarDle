

using System.Globalization;

namespace WarDle.Converters
{
    public class CharToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Convert char to string
            if (value is char c)
            {
                return c == '\0' ? string.Empty : c.ToString();
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && !string.IsNullOrEmpty(str))
            {
                return str[0];
            }
            return '\0'; // Return '\0' for invalid input
        }
    }
}
