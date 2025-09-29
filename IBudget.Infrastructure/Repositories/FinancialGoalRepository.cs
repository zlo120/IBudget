using IBudget.Core.Model;
using IBudget.Core.RepositoryInterfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace IBudget.Infrastructure.Repositories
{
    public class FinancialGoalRepository(MongoDbContext context) : IFinancialGoalRepository
    {
        private readonly IMongoCollection<FinancialGoal> _financialGoalsCollection = context.GetFinancialGoalsCollection();

        public async Task ClearCollection()
        {
            await _financialGoalsCollection.DeleteManyAsync(FilterDefinition<FinancialGoal>.Empty);
        }

        public async Task CreateFinancialGoal(FinancialGoal financialGoal)
        {
            await _financialGoalsCollection.InsertOneAsync(financialGoal);
        }

        public async Task DeleteFinancialGoalById(ObjectId id)
        {
            await _financialGoalsCollection.DeleteOneAsync(e => e.Id == id);
        }

        public async Task DeleteFinancialGoalByName(string name)
        {
            await _financialGoalsCollection.DeleteOneAsync(e => e.Name == name);
        }

        public async Task<List<FinancialGoal>> GetAll()
        {
            return await _financialGoalsCollection.Find(_ => true).ToListAsync();
        }

        public async Task<FinancialGoal> GetFinancialGoalByName(string name)
        {
            return await _financialGoalsCollection.Find(e => e.Name == name).FirstOrDefaultAsync();
        }

        public async Task UpdateFinancialGoal(FinancialGoal financialGoal)
        {
            await _financialGoalsCollection.ReplaceOneAsync(e => e.Id == financialGoal.Id, financialGoal);
        }
    }
}
