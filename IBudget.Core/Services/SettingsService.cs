using System.Text.Json;
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
        }
    }
}
