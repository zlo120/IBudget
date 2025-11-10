using IBudget.Core.Model;
using LiteDB;
using LiteDB.Async;
using MongoDB.Driver;

namespace IBudget.Infrastructure.Utils
{
    public static class ImportUtils
    {
        /// <summary>
        /// Imports data into MongoDB collection. 
        /// If an item with the same Id exists, it will be replaced. Otherwise, it will be inserted.
        /// Handles duplicate IDs in import data by keeping the last occurrence.
        /// </summary>
        public static async Task ImportCollectionIntoMongoDb<T>(IMongoCollection<T> collection, List<T> data) where T : BaseModel
        {
            if (data == null || data.Count == 0)
                return;

            var deduplicatedData = data
                .GroupBy(item => item.Id?.ToString() ?? Guid.NewGuid().ToString())
                .Select(group => group.Last())
                .ToList();

            var bulkOps = new List<WriteModel<T>>();

            foreach (var item in deduplicatedData)
            {
                if (item.Id == null || item.Id == MongoDB.Bson.ObjectId.Empty)
                {
                    item.Id = MongoDB.Bson.ObjectId.GenerateNewId();
                    bulkOps.Add(new InsertOneModel<T>(item));
                }
                else
                {
                    var filter = Builders<T>.Filter.Eq(x => x.Id, item.Id);
                    bulkOps.Add(new ReplaceOneModel<T>(filter, item) { IsUpsert = true });
                }
            }

            if (bulkOps.Count > 0)
            {
                var options = new BulkWriteOptions { IsOrdered = false };
                try
                {
                    await collection.BulkWriteAsync(bulkOps, options);
                }
                catch (MongoBulkWriteException ex)
                {
                    // Silently ignore duplicate key errors
                    var hasDuplicateKeyErrors = ex.WriteErrors.Any(e => e.Category == ServerErrorCategory.DuplicateKey);
                    if (!hasDuplicateKeyErrors || ex.WriteErrors.Count == bulkOps.Count)
                    {
                        // Only throw if there are non-duplicate-key errors or all operations failed
                        if (!hasDuplicateKeyErrors)
                            throw;
                    }
                }
            }
        }

        /// <summary>
        /// Imports data into LiteDB collection (async version).
        /// If an item with the same Id exists, it will be replaced. Otherwise, it will be inserted.
        /// Handles duplicate IDs in import data by keeping the last occurrence.
        /// </summary>
        public static async Task ImportCollectionIntoLiteDb<T>(ILiteCollectionAsync<T> collection, List<T> data) where T : BaseModel
        {
            if (data == null || data.Count == 0)
                return;

            var deduplicatedData = data
                .GroupBy(item => item.Id?.ToString() ?? Guid.NewGuid().ToString())
                .Select(group => group.Last())
                .ToList();

            foreach (var item in deduplicatedData)
            {
                try
                {
                    if (item.Id == null || item.Id == MongoDB.Bson.ObjectId.Empty)
                    {
                        item.Id = MongoDB.Bson.ObjectId.GenerateNewId();
                        await collection.InsertAsync(item);
                    }
                    else
                    {
                        var existingItem = await collection.FindByIdAsync(new BsonValue(item.Id.ToString()));
                        if (existingItem != null)
                        {
                            await collection.UpdateAsync(item);
                        }
                        else
                        {
                            await collection.InsertAsync(item);
                        }
                    }
                }
                catch (LiteAsyncException)
                {
                    continue;
                }
            }
        }
    }
}
