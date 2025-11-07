using IBudget.Core.Model;
using IBudget.Core.RepositoryInterfaces;
using LiteDB;
using MongoDB.Bson;

namespace IBudget.Infrastructure.Repositories.LiteDb
{
    public class LiteDbFinancialGoalRepository : IFinancialGoalRepository
    {
        private readonly ILiteCollection<FinancialGoal> _financialGoalsCollection;

        public LiteDbFinancialGoalRepository(LiteDbContext context)
        {
            _financialGoalsCollection = context.GetFinancialGoalsCollection();
            _financialGoalsCollection.EnsureIndex(f => f.Name);
        }

        public async Task ClearCollection()
        {
            await Task.Run(() => _financialGoalsCollection.DeleteAll());
        }

        public async Task CreateFinancialGoal(FinancialGoal financialGoal)
        {
            await Task.Run(() => _financialGoalsCollection.Insert(financialGoal));
        }

        public async Task DeleteFinancialGoalById(MongoDB.Bson.ObjectId id)
        {
            await Task.Run(() => _financialGoalsCollection.Delete(new LiteDB.BsonValue(id.ToString())));
        }

        public async Task DeleteFinancialGoalByName(string name)
        {
            await Task.Run(() => _financialGoalsCollection.DeleteMany(f => f.Name == name));
        }

        public async Task<List<FinancialGoal>> GetAll()
        {
            return await Task.Run(() => _financialGoalsCollection.FindAll().ToList());
        }

        public async Task<FinancialGoal> GetFinancialGoalByName(string name)
        {
            return await Task.Run(() => _financialGoalsCollection.FindOne(f => f.Name == name));
        }

        public async Task UpdateFinancialGoal(FinancialGoal financialGoal)
        {
            await Task.Run(() => _financialGoalsCollection.Update(financialGoal));
        }
    }
}
