using System.Threading.Tasks;
using IBudget.Core.Enums;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using IBudget.Core.Services;
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
            return Database.GetCollection<Tag>(DatabaseCollections.Tags);
        }

        public IMongoCollection<FinancialGoal> GetFinancialGoalsCollection()
        {
            return Database.GetCollection<FinancialGoal>(DatabaseCollections.FinancialGoals);
        }

        public static async Task TestConnection(string connectionString)
        {
            var client = new MongoClient(connectionString);
            await client.GetDatabase("admin").RunCommandAsync((Command<BsonDocument>)"{ping:1}");
        }
    }
}