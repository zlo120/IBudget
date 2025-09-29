using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace IBudget.GUI.Converters
{
    public class StringNotEqualConverter : IValueConverter
    {
        public static readonly StringNotEqualConverter Instance = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string stringValue && parameter is string parameterValue)
            {
                return !string.Equals(stringValue, parameterValue, StringComparison.OrdinalIgnoreCase);
            }
            
            return value?.ToString() != parameter?.ToString();
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}