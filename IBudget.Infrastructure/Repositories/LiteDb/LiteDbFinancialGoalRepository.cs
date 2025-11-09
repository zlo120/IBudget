using IBudget.Core.Model;
using IBudget.Core.RepositoryInterfaces;
using LiteDB;
using LiteDB.Async;

namespace IBudget.Infrastructure.Repositories.LiteDb
{
    public class LiteDbFinancialGoalRepository : IFinancialGoalRepository
    {
        private readonly ILiteCollectionAsync<FinancialGoal> _financialGoalsCollection;

        public LiteDbFinancialGoalRepository(LiteDbContext context)
        {
            _financialGoalsCollection = context.GetFinancialGoalsCollection();
            _financialGoalsCollection.EnsureIndexAsync(f => f.Name);
        }

        public async Task ClearCollection()
        {
            await _financialGoalsCollection.DeleteAllAsync();
        }

        public async Task CreateFinancialGoal(FinancialGoal financialGoal)
        {
            try
            {
                await _financialGoalsCollection.InsertAsync(financialGoal);
            }
            catch (LiteException ex) when (ex.ErrorCode == LiteException.INDEX_DUPLICATE_KEY)
            {
                // Silently ignore duplicate key errors
            }
        }

        public async Task DeleteFinancialGoalById(MongoDB.Bson.ObjectId id)
        {
            await _financialGoalsCollection.DeleteAsync(new BsonValue(id.ToString()));
        }

        public async Task DeleteFinancialGoalByName(string name)
        {
            await _financialGoalsCollection.DeleteManyAsync(f => f.Name == name);
        }

        public async Task<List<FinancialGoal>> GetAll()
        {
            return [.. await _financialGoalsCollection.FindAllAsync()] ;
        }

        public async Task<FinancialGoal> GetFinancialGoalByName(string name)
        {
            return await _financialGoalsCollection.FindOneAsync(f => f.Name == name);
        }

        public async Task UpdateFinancialGoal(FinancialGoal financialGoal)
        {
            await _financialGoalsCollection.UpdateAsync(financialGoal);
        }
    }
}
