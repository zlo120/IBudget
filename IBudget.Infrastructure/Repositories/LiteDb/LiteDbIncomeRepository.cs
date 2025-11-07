using IBudget.Core.Model;
using IBudget.Core.RepositoryInterfaces;
using LiteDB;
using MongoDB.Bson;

namespace IBudget.Infrastructure.Repositories.LiteDb
{
    public class LiteDbIncomeRepository : IIncomeRepository
    {
        private readonly ILiteCollection<Income> _incomeCollection;

        public LiteDbIncomeRepository(LiteDbContext context)
        {
            _incomeCollection = context.GetIncomeCollection();
            _incomeCollection.EnsureIndex(i => i.Date);
            _incomeCollection.EnsureIndex(i => i.BatchHash);
        }

        public async Task AddIncome(Income income)
        {
            await Task.Run(() => _incomeCollection.Insert(income));
        }

        public async Task ClearCollection()
        {
            await Task.Run(() => _incomeCollection.DeleteAll());
        }

        public async Task DeleteIncome(MongoDB.Bson.ObjectId id)
        {
            await Task.Run(() => _incomeCollection.Delete(new LiteDB.BsonValue(id.ToString())));
        }

        public async Task<bool> DoesBatchHashExist(string batchHash)
        {
            return await Task.Run(() => _incomeCollection.Exists(i => i.BatchHash == batchHash));
        }

        public async Task<Income> GetIncome(MongoDB.Bson.ObjectId id)
        {
            return await Task.Run(() => _incomeCollection.FindById(new LiteDB.BsonValue(id.ToString())));
        }

        public async Task<List<Income>> GetIncomeByMonth(int month)
        {
            var startDate = new DateTime(DateTime.Today.Year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            return await Task.Run(() => 
                _incomeCollection.Find(i => i.Date >= startDate && i.Date <= endDate).ToList());
        }

        public async Task<List<Income>> GetIncomeByWeek(DateTime startDate)
        {
            var endDate = startDate.AddDays(6);
            return await Task.Run(() => 
                _incomeCollection.Find(i => i.Date >= startDate && i.Date <= endDate).ToList());
        }

        public async Task UpdateIncome(Income income)
        {
            await Task.Run(() => _incomeCollection.Update(income));
        }
    }
}
