using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using IBudget.Core.RepositoryInterfaces;
using MongoDB.Bson;

namespace IBudget.Core.Services
{
    public class FinancialGoalService(IFinancialGoalRepository financialGoalRepository) : IFinancialGoalService
    {
        private readonly IFinancialGoalRepository _financialGoalRepository = financialGoalRepository;

        public async Task ClearCollection()
        {
            await _financialGoalRepository.ClearCollection();
        }

        public async Task CreateFinancialGoal(FinancialGoal financialGoal)
        {
            await _financialGoalRepository.CreateFinancialGoal(financialGoal);
        }

        public async Task DeleteFinancialGoalById(ObjectId id)
        {
            await _financialGoalRepository.DeleteFinancialGoalById(id);
        }

        public async Task DeleteFinancialGoalByName(string name)
        {
            await _financialGoalRepository.DeleteFinancialGoalByName(name);
        }

        public async Task<List<FinancialGoal>> GetAll()
        {
            return await _financialGoalRepository.GetAll();
        }

        public async Task<FinancialGoal> GetFinancialGoalByName(string name)
        {
            return await _financialGoalRepository.GetFinancialGoalByName(name);
        }

        public async Task UpdateFinancialGoal(FinancialGoal financialGoal)
        {
            await _financialGoalRepository.UpdateFinancialGoal(financialGoal);
        }
    }
}
