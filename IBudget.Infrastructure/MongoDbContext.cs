using IBudget.Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Tag = IBudget.Core.Model.Tag;

namespace IBudget.Infrastructure
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        public MongoDbContext(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("MongoDB"));
            _database = client.GetDatabase("Stacks");
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
    }
}