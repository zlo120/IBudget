using IBudget.Core.Model;
using MongoDB.Bson;

namespace IBudget.Core.RepositoryInterfaces
{
    public interface IFinancialGoalRepository
    {
        Task<List<FinancialGoal>> GetAll();
        Task<FinancialGoal> GetFinancialGoalByName(string name);
        Task CreateFinancialGoal(FinancialGoal financialGoal);
        Task DeleteFinancialGoalByName(string name);
        Task DeleteFinancialGoalById(ObjectId id);
        Task UpdateFinancialGoal(FinancialGoal financialGoal);
        Task ClearCollection();
    }
}
