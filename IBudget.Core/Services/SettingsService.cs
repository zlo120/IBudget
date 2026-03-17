using System.Text.Json;
using IBudget.Core.Enums;
using IBudget.Core.Interfaces;

namespace IBudget.Core.Services
{
    public class SettingsService : ISettingsService
    {
        private static readonly string _directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Stacks");
        private static readonly string _path = Path.Combine(_directory, "appsettings.json");
        private string GetValueFromSettings(string key)
        {
            if (!File.Exists(_path))
            {
                throw new FileNotFoundException($"The file {_path} does not exist.");
            }

            var file = File.ReadAllText(_path);
            var settings = JsonSerializer.Deserialize<Dictionary<string, string>>(file, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            }) ?? throw new InvalidOperationException("Failed to deserialize settings.");

            if (settings.TryGetValue(key, out var value))
            {
                return value;
            }

            throw new KeyNotFoundException($"The key '{key}' was not found in the settings.");
        }
        private void SetValueInSettings(string key, string? value)
        {
            Dictionary<string, string> settings;

            // Read existing settings if file exists
            if (File.Exists(_path))
            {
                var file = File.ReadAllText(_path);
                settings = JsonSerializer.Deserialize<Dictionary<string, string>>(file, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    WriteIndented = true
                }) ?? new Dictionary<string, string>();
            }
            else
            {
                settings = new Dictionary<string, string>();
            }

            // Add or update the key-value pair
            settings[key] = value ?? null;

            // Serialize and write back to file
            var updatedJson = JsonSerializer.Serialize(settings, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            });

            if (!Directory.Exists(_directory))
            {
                Directory.CreateDirectory(_directory);
            }

            File.WriteAllText(_path, updatedJson);
        }    
        public string GetDbConnectionString()
        {
            return GetValueFromSettings("ConnectionString");
        }

        public void SetDbConnectionString(string connectionString)
        {
            SetValueInSettings("ConnectionString", connectionString);
            SetDatabaseType(DatabaseType.CustomMongoDbInstance);
        }

        public void ResetDbConnectionString()
        {
            SetValueInSettings("ConnectionString", null);
            SetDatabaseType(DatabaseType.CustomMongoDbInstance);
        }

        public void SetDatabaseType(DatabaseType? databaseType)
        {
            SetValueInSettings("DatabaseType", databaseType?.ToString() ?? DatabaseType.None.ToString());
        }

        public DatabaseType? GetDatabaseType()
        {
            var value = GetValueFromSettings("DatabaseType");
            if (Enum.TryParse<DatabaseType>(value, out var dbType))
            {
                return dbType == DatabaseType.None ? null : dbType;
            }
            return null;
        }
    
        public string? GetTheme()
        {
            try
            {
                return GetValueFromSettings("Theme");
            }
            catch
            {
                return null;
            }
        }

        public void SetTheme(string theme)
        {
            SetValueInSettings("Theme", theme);
        }
    }
}
