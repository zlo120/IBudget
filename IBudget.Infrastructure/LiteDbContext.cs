using IBudget.Core.Enums;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using LiteDB;
using Tag = IBudget.Core.Model.Tag;

namespace IBudget.Infrastructure
{
    public class LiteDbContext : IDisposable
    {
        private readonly Lazy<LiteDatabase> _database;
        private readonly ISettingsService _settingsService;
        private readonly string _databasePath;

        public LiteDbContext(ISettingsService settingsService)
        {
            _settingsService = settingsService;
            _databasePath = GetLiteDbPath();
            
            // Use lazy initialization to defer the LiteDatabase creation
            // until it's actually needed
            _database = new Lazy<LiteDatabase>(() => new LiteDatabase(_databasePath));
        }

        private string GetLiteDbPath()
        {
            // Default to local database file
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var dbDirectory = Path.Combine(appDataPath, "Stacks");
            Directory.CreateDirectory(dbDirectory);
            return Path.Combine(dbDirectory, "Stacks.db");
        }

        private LiteDatabase Database => _database.Value;

        public ILiteCollection<ExpenseRuleTag> GetExpenseRuleTagsCollection()
        {
            return Database.GetCollection<ExpenseRuleTag>("expenseRuleTags");
        }

        public ILiteCollection<ExpenseTag> GetExpenseTagsCollection()
        {
            return Database.GetCollection<ExpenseTag>("expenseTags");
        }

        public ILiteCollection<Expense> GetExpensesCollection()
        {
            return Database.GetCollection<Expense>("expenses");
        }

        public ILiteCollection<Income> GetIncomeCollection()
        {
            return Database.GetCollection<Income>("income");
        }

        public ILiteCollection<Tag> GetTagsCollection()
        {
            return Database.GetCollection<Tag>("tags");
        }

        public ILiteCollection<FinancialGoal> GetFinancialGoalsCollection()
        {
            return Database.GetCollection<FinancialGoal>("financialGoals");
        }

        public void Dispose()
        {
            if (_database.IsValueCreated)
            {
                _database.Value?.Dispose();
            }
        }
    }
}
