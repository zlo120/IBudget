using IBudget.Core.Model;
using IBudget.Core.RepositoryInterfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace IBudget.Infrastructure.Repositories
{
    public class ExpensesRepository(MongoDbContext context) : IExpenseRepository
    {
        private readonly IMongoCollection<Expense> _expensesCollection = context.GetExpensesCollection();
        
        public async Task AddExpense(Expense expense)
        {
            try
            {
                await _expensesCollection.InsertOneAsync(expense);
            }
            catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
            {
                // Silently ignore duplicate key errors
            }
        }

        public async Task ClearCollection()
        {
            await _expensesCollection.DeleteManyAsync(FilterDefinition<Expense>.Empty);
        }

        public async Task DeleteExpense(ObjectId id)
        {
            await _expensesCollection.DeleteOneAsync(e => e.Id == id);
        }

        public async Task<bool> DoesBatchHashExist(string batchHash)
        {
            return await _expensesCollection.Find(e => e.BatchHash == batchHash).AnyAsync();
        }

        public async Task<Expense> GetExpense(ObjectId id)
        {
            return await _expensesCollection.Find(e => e.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Expense>> GetExpenseByWeek(DateTime startDate)
        {
            var endDate = startDate.AddDays(6);
            var filter = Builders<Expense>.Filter.And(
                Builders<Expense>.Filter.Gte(e => e.Date, startDate),
                Builders<Expense>.Filter.Lte(e => e.Date, endDate)
            );
            return await _expensesCollection.Find(filter).ToListAsync();
        }

        public async Task<List<Expense>> GetExpensesByMonth(int month)
        {
            var startDate = new DateTime(DateTime.Today.Year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var filter = Builders<Expense>.Filter.And(
                Builders<Expense>.Filter.Gte(e => e.Date, startDate),
                Builders<Expense>.Filter.Lte(e => e.Date, endDate)
            );
            return await _expensesCollection.Find(filter).ToListAsync();
        }

        public async Task UpdateExpense(Expense expense)
        {
            await _expensesCollection.ReplaceOneAsync(e => e.Id == expense.Id, expense);
        }
    }
}
