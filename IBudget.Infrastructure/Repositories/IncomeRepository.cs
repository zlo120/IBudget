using IBudget.Core.Model;
using IBudget.Core.RepositoryInterfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace IBudget.Infrastructure.Repositories
{
    public class IncomeRepository(MongoDbContext context) : IIncomeRepository
    {
        private readonly IMongoCollection<Income> _incomeCollection = context.GetIncomeCollection();
        public async Task AddIncome(Income income)
        {
            await _incomeCollection.InsertOneAsync(income);
        }

        public async Task ClearCollection()
        {
            await _incomeCollection.DeleteManyAsync(FilterDefinition<Income>.Empty);
        }

        public async Task DeleteIncome(ObjectId id)
        {
            await _incomeCollection.DeleteOneAsync(e => e.Id == id);
        }

        public async Task<bool> DoesBatchHashExist(string batchHash)
        {
            return await _incomeCollection.Find(e => e.BatchHash == batchHash).AnyAsync();
        }

        public async Task<Income> GetIncome(ObjectId id)
        {
            return await _incomeCollection.Find(e => e.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Income>> GetIncomeByMonth(int month)
        {
            var startDate = new DateTime(DateTime.Today.Year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var filter = Builders<Income>.Filter.And(
                Builders<Income>.Filter.Gte(e => e.Date, startDate),
                Builders<Income>.Filter.Lte(e => e.Date, endDate)
            );
            return await _incomeCollection.Find(filter).ToListAsync();
        }

        public async Task<List<Income>> GetIncomeByWeek(DateTime startDate)
        {
            var endDate = startDate.AddDays(6);
            var filter = Builders<Income>.Filter.And(
                Builders<Income>.Filter.Gte(e => e.Date, startDate),
                Builders<Income>.Filter.Lte(e => e.Date, endDate)
            );
            return await _incomeCollection.Find(filter).ToListAsync();
        }

        public async Task UpdateIncome(Income income)
        {
            await _incomeCollection.ReplaceOneAsync(e => e.Id == income.Id, income);
        }
    }
}
