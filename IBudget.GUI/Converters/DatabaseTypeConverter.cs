using System;
using System.Globalization;
using Avalonia.Data.Converters;
using IBudget.Core.Enums;

namespace IBudget.GUI.Converters
{
    public class DatabaseTypeConverter : IValueConverter
    {
        public static readonly DatabaseTypeConverter Instance = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DatabaseType databaseType)
            {
                return databaseType switch
                {
                    DatabaseType.CustomMongoDbInstance => "Custom MongoDB Instance",
                    DatabaseType.Offline => "Offline",
                    DatabaseType.StacksBackend => "Stacks Backend",
                    _ => databaseType.ToString()
                };
            }
            return value?.ToString();
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                return stringValue switch
                {
                    "Custom MongoDB Instance" => DatabaseType.CustomMongoDbInstance,
                    "Offline" => DatabaseType.Offline,
                    "Stacks Backend" => DatabaseType.StacksBackend,
                    _ => DatabaseType.CustomMongoDbInstance
                };
            }
            return DatabaseType.CustomMongoDbInstance;
        }
    }
}