using IBudget.Core.Enums;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using LiteDB;
using LiteDB.Async;
using Tag = IBudget.Core.Model.Tag;

namespace IBudget.Infrastructure
{
    public class LiteDbContext : IDisposable
    {
        private readonly Lazy<LiteDatabaseAsync> _database;
        private readonly string _databasePath;
        private bool _disposed = false;

        public LiteDbContext()
        {
            _databasePath = GetLiteDbPath();

            // Use lazy initialization to defer the LiteDatabase creation
            // until it's actually needed
            _database = new Lazy<LiteDatabaseAsync>(() => new LiteDatabaseAsync(_databasePath));
        }

        private string GetLiteDbPath()
        {
            // Default to local database file
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var dbDirectory = Path.Combine(appDataPath, "Stacks");
            Directory.CreateDirectory(dbDirectory);
            return Path.Combine(dbDirectory, "Stacks.db");
        }

        private LiteDatabaseAsync Database => _database.Value;

        public ILiteCollectionAsync<ExpenseRuleTag> GetExpenseRuleTagsCollection()
        {
            return Database.GetCollection<ExpenseRuleTag>(DatabaseCollections.ExpenseRuleTags);
        }

        public ILiteCollectionAsync<ExpenseTag> GetExpenseTagsCollection()
        {
            return Database.GetCollection<ExpenseTag>(DatabaseCollections.ExpenseTags);
        }

        public ILiteCollectionAsync<Expense> GetExpensesCollection()
        {
            return Database.GetCollection<Expense>(DatabaseCollections.Expenses);
        }

        public ILiteCollectionAsync<Income> GetIncomeCollection()
        {
            return Database.GetCollection<Income>(DatabaseCollections. Income);
        }

        public ILiteCollectionAsync<Tag> GetTagsCollection()
        {
            var collection = Database.GetCollection<Tag>(DatabaseCollections.Tags);
            collection.EnsureIndexAsync(t => t.Name, true);
            return collection;
        }

        public ILiteCollectionAsync<FinancialGoal> GetFinancialGoalsCollection()
        {
            var collection = Database.GetCollection<FinancialGoal>(DatabaseCollections.FinancialGoals);
            collection.EnsureIndexAsync(fg => fg.Name, true);
            return collection;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (_database.IsValueCreated)
                {
                    try
                    {
                        // LiteDatabaseAsync implements IDisposable
                        _database.Value?.Dispose();
                        
                        // Give a small delay to allow background threads to complete
                        System.Threading.Thread.Sleep(100);
                    }
                    catch (Exception)
                    {
                        // Suppress any exceptions during disposal
                    }
                }
            }

            _disposed = true;
        }
    }
}
