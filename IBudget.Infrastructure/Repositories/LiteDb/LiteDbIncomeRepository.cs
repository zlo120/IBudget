using IBudget.Core.Model;
using IBudget.Core.RepositoryInterfaces;
using LiteDB;
using LiteDB.Async;

namespace IBudget.Infrastructure.Repositories.LiteDb
{
    public class LiteDbIncomeRepository : IIncomeRepository
    {
        private readonly ILiteCollectionAsync<Income> _incomeCollection;

        public LiteDbIncomeRepository(LiteDbContext context)
        {
            _incomeCollection = context.GetIncomeCollection();
            _incomeCollection.EnsureIndexAsync(i => i.Date);
            _incomeCollection.EnsureIndexAsync(i => i.BatchHash);
        }

        public async Task AddIncome(Income income)
        {
            try
            {
                await Task.Run(() => _incomeCollection.InsertAsync(income));
            }
            catch (LiteException ex) when (ex.ErrorCode == LiteException.INDEX_DUPLICATE_KEY)
            {
                // Silently ignore duplicate key errors
            }
        }

        public async Task ClearCollection()
        {
            await _incomeCollection.DeleteAllAsync();
        }

        public async Task DeleteIncome(MongoDB.Bson.ObjectId id)
        {
            await _incomeCollection.DeleteAsync(new LiteDB.BsonValue(id.ToString()));
        }

        public async Task<bool> DoesBatchHashExist(string batchHash)
        {
            return await _incomeCollection.ExistsAsync(i => i.BatchHash == batchHash);
        }

        public async Task<Income> GetIncome(MongoDB.Bson.ObjectId id)
        {
            return await _incomeCollection.FindByIdAsync(new LiteDB.BsonValue(id.ToString()));
        }

        public async Task<List<Income>> GetIncomeByMonth(int month)
        {
            var startDate = new DateTime(DateTime.Today.Year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            return [.. await _incomeCollection.FindAsync(i => i.Date >= startDate && i.Date <= endDate)];
        }

        public async Task<List<Income>> GetIncomeByWeek(DateTime startDate)
        {
            var endDate = startDate.AddDays(6);
            return [.. await _incomeCollection.FindAsync(i => i.Date >= startDate && i.Date <= endDate)];
        }

        public async Task UpdateIncome(Income income)
        {
            await _incomeCollection.UpdateAsync(income);
        }
    }
}
