using IBudget.Core.Enums;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using Tag = IBudget.Core.Model.Tag;

namespace IBudget.Infrastructure
{
    public class MongoDbContext
    {
        private readonly Lazy<IMongoDatabase> _database;
        private readonly ISettingsService _settingsService;

        public MongoDbContext(ISettingsService settingsService)
        {
            _settingsService = settingsService;

            // Use lazy initialization to defer the blocking MongoClient creation
            // until the database is actually needed
            _database = new Lazy<IMongoDatabase>(() =>
            {
                var connectionString = _settingsService.GetDbConnectionString();
                var client = new MongoClient(connectionString);
                return client.GetDatabase("Stacks");
            });
        }

        private IMongoDatabase Database => _database.Value;

        public IMongoCollection<ExpenseRuleTag> GetExpenseRuleTagsCollection()
        {
            return Database.GetCollection<ExpenseRuleTag>(DatabaseCollections.ExpenseRuleTags);
        }

        public IMongoCollection<ExpenseTag> GetExpenseTagsCollection()
        {
            return Database.GetCollection<ExpenseTag>(DatabaseCollections.ExpenseTags);
        }

        public IMongoCollection<Expense> GetExpensesCollection()
        {
            return Database.GetCollection<Expense>(DatabaseCollections.Expenses);
        }

        public IMongoCollection<Income> GetIncomeCollection()
        {
            return Database.GetCollection<Income>(DatabaseCollections.Income);
        }

        public IMongoCollection<Tag> GetTagsCollection()
        {
            var collection = Database.GetCollection<Tag>(DatabaseCollections.Tags);
            EnsureIndexExists(collection, t => t.Name, "Name_1", true);
            return collection;
        }

        public IMongoCollection<FinancialGoal> GetFinancialGoalsCollection()
        {
            var collection = Database.GetCollection<FinancialGoal>(DatabaseCollections.FinancialGoals);
            EnsureIndexExists(collection, fg => fg.Name, "Name_1", true);
            return collection;
        }

        private void EnsureIndexExists<T>(IMongoCollection<T> collection, System.Linq.Expressions.Expression<Func<T, object>> field, string indexName, bool unique = false)
        {
            // Check if index already exists
            var indexes = collection.Indexes.List().ToList();
            var indexExists = indexes.Any(idx => 
            {
                var name = idx.GetValue("name", null);
                return name != null && name.AsString == indexName;
            });

            if (!indexExists)
            {
                var indexKeysDefinition = Builders<T>.IndexKeys.Ascending(field);
                var indexOptions = new CreateIndexOptions { Unique = unique };
                var indexModel = new CreateIndexModel<T>(indexKeysDefinition, indexOptions);
                collection.Indexes.CreateOne(indexModel);
            }
        }

        public static async Task TestConnection(string connectionString)
        {
            var client = new MongoClient(connectionString);
            await client.GetDatabase("admin").RunCommandAsync((Command<BsonDocument>)"{ping:1}");
        }
    }
}