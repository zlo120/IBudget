using System.Text.Json;
using IBudget.Core.Enums;
using IBudget.Core.Interfaces;

namespace IBudget.Core.Services
{
    public class SettingsService : ISettingsService
    {
        private static readonly string _directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Stacks");
        private static readonly string _path = Path.Combine(_directory, "appsettings.json");

        public string GetDbConnectionString()
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

            if (settings.TryGetValue("ConnectionString", out var connectionString))
            {
                return connectionString;
            }

            throw new KeyNotFoundException("The key 'ConnectionString' was not found in the settings.");
        }

        public void SetDbConnectionString(string connectionString)
        {
            var setting = JsonSerializer.Serialize(new Dictionary<string, string>
            {
                { "ConnectionString", connectionString }
            }, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            });

            if (!Directory.Exists(_directory))
            {
                Directory.CreateDirectory(_directory);
            }

            File.WriteAllText(_path, setting);

            SetDatabaseType(DatabaseType.CustomMongoDbInstance);
        }

        public void ResetDbConnectionString()
        {
            var setting = JsonSerializer.Serialize(new Dictionary<string, string>
            {
                { "ConnectionString", null }
            }, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            });

            if (!Directory.Exists(_directory))
            {
                Directory.CreateDirectory(_directory);
            }

            File.WriteAllText(_path, setting);
            SetDatabaseType(DatabaseType.CustomMongoDbInstance);
        }

        public void SetDatabaseType(DatabaseType? databaseType)
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

            // Add or update the DatabaseType
            settings["DatabaseType"] = databaseType.ToString() ?? DatabaseType.None.ToString();

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

        public DatabaseType? GetDatabaseType()
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

            if (settings.TryGetValue("DatabaseType", out var databaseType))
            {
                var dbType = (DatabaseType)Enum.Parse(typeof(DatabaseType), databaseType);
                if (dbType == DatabaseType.None)
                {
                    return null;
                }
                return dbType;
            }

            throw new KeyNotFoundException("The key 'DatabaseType' was not found in the settings.");
        }
    }
}
