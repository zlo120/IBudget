using IBudget.Core.Enums;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using LiteDB;
using Tag = IBudget.Core.Model.Tag;

namespace IBudget.Infrastructure
{
    public class LiteDbContext : IDisposable
    {
        private LiteDatabase _database;
        private readonly ISettingsService _settingsService;
        private string _databasePath;

        public LiteDbContext(ISettingsService settingsService)
        {
            _settingsService = settingsService;
            _databasePath = GetLiteDbPath();
            _database = new LiteDatabase(_databasePath);
        }

        private string GetLiteDbPath()
        {
            // Default to local database file
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var dbDirectory = Path.Combine(appDataPath, "Stacks");
            Directory.CreateDirectory(dbDirectory);
            return Path.Combine(dbDirectory, "Stacks.db");
        }

        public ILiteCollection<ExpenseRuleTag> GetExpenseRuleTagsCollection()
        {
            return _database.GetCollection<ExpenseRuleTag>("expenseRuleTags");
        }

        public ILiteCollection<ExpenseTag> GetExpenseTagsCollection()
        {
            return _database.GetCollection<ExpenseTag>("expenseTags");
        }

        public ILiteCollection<Expense> GetExpensesCollection()
        {
            return _database.GetCollection<Expense>("expenses");
        }

        public ILiteCollection<Income> GetIncomeCollection()
        {
            return _database.GetCollection<Income>("income");
        }

        public ILiteCollection<Tag> GetTagsCollection()
        {
            return _database.GetCollection<Tag>("tags");
        }

        public ILiteCollection<FinancialGoal> GetFinancialGoalsCollection()
        {
            return _database.GetCollection<FinancialGoal>("financialGoals");
        }

        public void Dispose()
        {
            _database?.Dispose();
        }
    }
}
