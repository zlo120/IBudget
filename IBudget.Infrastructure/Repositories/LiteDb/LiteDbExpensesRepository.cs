using IBudget.Core.Model;
using IBudget.Core.RepositoryInterfaces;
using LiteDB;

namespace IBudget.Infrastructure.Repositories.LiteDb
{
    public class LiteDbExpensesRepository : IExpenseRepository
    {
        private readonly ILiteCollection<Expense> _expensesCollection;

        public LiteDbExpensesRepository(LiteDbContext context)
        {
            _expensesCollection = context.GetExpensesCollection();
            _expensesCollection.EnsureIndex(e => e.Date);
            _expensesCollection.EnsureIndex(e => e.BatchHash);
        }

        public async Task AddExpense(Expense expense)
        {
            await Task.Run(() => _expensesCollection.Insert(expense));
        }

        public async Task ClearCollection()
        {
            await Task.Run(() => _expensesCollection.DeleteAll());
        }

        public async Task DeleteExpense(MongoDB.Bson.ObjectId id)
        {
            await Task.Run(() => _expensesCollection.Delete(new LiteDB.BsonValue(id.ToString())));
        }

        public async Task<bool> DoesBatchHashExist(string batchHash)
        {
            return await Task.Run(() => _expensesCollection.Exists(e => e.BatchHash == batchHash));
        }

        public async Task<Expense> GetExpense(MongoDB.Bson.ObjectId id)
        {
            return await Task.Run(() => _expensesCollection.FindById(new LiteDB.BsonValue(id.ToString())));
        }

        public async Task<List<Expense>> GetExpenseByWeek(DateTime startDate)
        {
            var endDate = startDate.AddDays(6);
            return await Task.Run(() => 
                _expensesCollection.Find(e => e.Date >= startDate && e.Date <= endDate).ToList());
        }

        public async Task<List<Expense>> GetExpensesByMonth(int month)
        {
            var startDate = new DateTime(DateTime.Today.Year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            return await Task.Run(() => 
                _expensesCollection.Find(e => e.Date >= startDate && e.Date <= endDate).ToList());
        }

        public async Task UpdateExpense(Expense expense)
        {
            await Task.Run(() => _expensesCollection.Update(expense));
        }
    }
}
