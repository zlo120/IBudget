using IBudget.Core.Model;
using IBudget.Core.RepositoryInterfaces;
using LiteDB;
using LiteDB.Async;

namespace IBudget.Infrastructure.Repositories.LiteDb
{
    public class LiteDbExpensesRepository : IExpenseRepository
    {
        private readonly ILiteCollectionAsync<Expense> _expensesCollection;

        public LiteDbExpensesRepository(LiteDbContext context)
        {
            _expensesCollection = context.GetExpensesCollection();
            _expensesCollection.EnsureIndexAsync(e => e.Date);
            _expensesCollection.EnsureIndexAsync(e => e.BatchHash);
        }

        public async Task AddExpense(Expense expense)
        {
            try
            {
                await _expensesCollection.InsertAsync(expense);
            }
            catch (LiteException ex) when (ex.ErrorCode == LiteException.INDEX_DUPLICATE_KEY)
            {
                // Silently ignore duplicate key errors
            }
        }

        public async Task ClearCollection()
        {
            await _expensesCollection.DeleteAllAsync();
        }

        public async Task DeleteExpense(MongoDB.Bson.ObjectId id)
        {
            await _expensesCollection.DeleteAsync(new LiteDB.BsonValue(id.ToString()));
        }

        public async Task<bool> DoesBatchHashExist(string batchHash)
        {
            return await _expensesCollection.ExistsAsync(e => e.BatchHash == batchHash);
        }

        public async Task<Expense> GetExpense(MongoDB.Bson.ObjectId id)
        {
            return await _expensesCollection.FindByIdAsync(new LiteDB.BsonValue(id.ToString()));
        }

        public async Task<List<Expense>> GetExpenseByWeek(DateTime startDate)
        {
            var endDate = startDate.AddDays(6);
            return [.. await _expensesCollection.FindAsync(e => e.Date >= startDate && e.Date <= endDate)];
        }

        public async Task<List<Expense>> GetExpensesByMonth(int month)
        {
            var startDate = new DateTime(DateTime.Today.Year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            return [.. await _expensesCollection.FindAsync(e => e.Date >= startDate && e.Date <= endDate)];
        }

        public async Task UpdateExpense(Expense expense)
        {
            await _expensesCollection.UpdateAsync(expense);
        }
    }
}
