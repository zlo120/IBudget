using IBudget.Core.Model;
using MongoDB.Bson;

namespace IBudget.Core.Interfaces
{
    public interface IFinancialGoalService
    {
        Task<List<FinancialGoal>> GetAll();
        Task<FinancialGoal> GetFinancialGoalByName(string name);
        Task CreateFinancialGoal(FinancialGoal financialGoal);
        Task DeleteFinancialGoalByName(string name);
        Task DeleteFinancialGoalById(ObjectId id);
        Task UpdateFinancialGoal(FinancialGoal financialGoal);
    }
}
