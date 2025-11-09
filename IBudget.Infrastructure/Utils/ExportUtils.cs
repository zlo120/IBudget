using IBudget.Core.Model;
using LiteDB;
using LiteDB.Async;
using MongoDB.Driver;

namespace IBudget.Infrastructure.Utils
{
    public static class ExportUtils
    {
        public static string GetExportCollectionPath(string databaseName, string timestamp)
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var stacksPath = Path.Combine(appDataPath, "Stacks");
            if (!Directory.Exists(stacksPath))
            {
                Directory.CreateDirectory(stacksPath);
            }
            var exportFolderPath = Path.Combine(stacksPath, "Exports", databaseName, timestamp);
            if (!Directory.Exists(exportFolderPath))
            {
                Directory.CreateDirectory(exportFolderPath);
            }

            return exportFolderPath;
        }
        public static string GetExportCollectionFilePath(string collectionName, string timestamp, string databaseName)
        {
            var exportDirectory = GetExportCollectionPath(databaseName, timestamp);
            var fileName = $"{collectionName}_Export_{timestamp}.json";
            return Path.Combine(exportDirectory, fileName);
        }

        public static async Task ExportMongoCollectionToFile<T>(string collectionName, string timestamp, IMongoCollection<T> collection)
        {
            var exportFilePath = GetExportCollectionFilePath(collectionName, timestamp, "MongoDb");
            var allData = await collection.Find(_ => true).ToListAsync();
            var exportData = new ExportFile<T>() { CollectionName = collectionName, Data = allData };
            var jsonData = System.Text.Json.JsonSerializer.Serialize(exportData, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = 
                { 
                    new ObjectIdJsonConverter(),
                    new NullableObjectIdJsonConverter()
                }
            });
            await File.WriteAllTextAsync(exportFilePath, jsonData);
        }

        public static async Task ExportLiteDbCollectionToFileAsync<T>(string collectionName, string timestamp, ILiteCollectionAsync<T> collection)
        {
            var exportFilePath = GetExportCollectionFilePath(collectionName, timestamp, "LiteDb");
            var allData = (await collection.FindAllAsync()).ToList();
            var exportData = new ExportFile<T>() { CollectionName = collectionName, Data = allData };
            var jsonData = System.Text.Json.JsonSerializer.Serialize(exportData, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true,
                Converters =
                {
                    new ObjectIdJsonConverter(),
                    new NullableObjectIdJsonConverter()
                }
            });
            File.WriteAllText(exportFilePath, jsonData);
        }
    }
}
