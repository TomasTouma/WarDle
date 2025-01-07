

using System.Globalization;

namespace WarDle.Converters
{
    // This class converts a character to a string and vice versa
    public class CharToStringConverter : IValueConverter
    {
        // This method converts a char to a string
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Convert char to string
            if (value is char c)
            {
                return c == '\0' ? string.Empty : c.ToString();
            }

            return string.Empty;
        }

        // This method converts a string back to a char
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
