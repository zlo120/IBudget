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
        private IMongoDatabase _database;
        private readonly ISettingsService _settingsService;
        public MongoDbContext(ISettingsService settingsService)
        {
            _settingsService = settingsService;
            var connectionString = settingsService.GetDbConnectionString();
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase("Stacks");
        }

        public void ChangeDatabase(DatabaseType databaseType)
        {
            switch(databaseType)
            {
                case DatabaseType.Offline:
                    throw new NotImplementedException("Offline database mode is not implemented yet.");
                case DatabaseType.StacksBackend:
                    throw new NotImplementedException("StacksBackend database mode is not implemented yet.");
                case DatabaseType.CustomMongoDbInstance:
                    var connectionString = _settingsService.GetDbConnectionString();
                    var client = new MongoClient(connectionString);
                    _database = client.GetDatabase("Stacks");
                    break;
            }
        }

        public IMongoCollection<ExpenseRuleTag> GetExpenseRuleTagsCollection()
        {
            return _database.GetCollection<ExpenseRuleTag>("expenseRuleTags");
        }

        public IMongoCollection<ExpenseTag> GetExpenseTagsCollection()
        {

            return _database.GetCollection<ExpenseTag>("expenseTags");
        }

        public IMongoCollection<Expense> GetExpensesCollection()
        {
            return _database.GetCollection<Expense>("expenses");
        }

        public IMongoCollection<Income> GetIncomeCollection()
        {
            return _database.GetCollection<Income>("income");
        }

        public IMongoCollection<Tag> GetTagsCollection()
        {
            return _database.GetCollection<Tag>("tags");
        }

        public IMongoCollection<FinancialGoal> GetFinancialGoalsCollection()
        {
            return _database.GetCollection<FinancialGoal>("financialGoals");
        }

        public static async Task TestConnection(string connectionString)
        {
            var client = new MongoClient(connectionString);
            await client.GetDatabase("admin").RunCommandAsync((Command<BsonDocument>)"{ping:1}");
        }
    }
}