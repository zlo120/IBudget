using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using IBudget.Core.RepositoryInterfaces;
using MongoDB.Bson;

namespace IBudget.Core.Services
{
    public class IncomeService : IIncomeService
    {
        private readonly IIncomeRepository _incomeRepository;
        public IncomeService(IIncomeRepository incomeRepository)
        {
            _incomeRepository = incomeRepository;
        }

        public async Task AddIncome(Income income)
        {
            for (int i = 0; i < income.Tags.Count; i++)
            {
                var tag = income.Tags[i];
                tag.Name = tag.Name.ToLower();
                income.Tags[i] = tag;
            }

            await _incomeRepository.AddIncome(income);
        }

        public async Task ClearCollection()
        {
            await _incomeRepository.ClearCollection();
        }

        public async Task DeleteIncome(ObjectId id)
        {
            await _incomeRepository.DeleteIncome(id);
        }

        public async Task<bool> DoesBatchHashExist(string batchHash)
        {
            return await _incomeRepository.DoesBatchHashExist(batchHash);  
        }

        public async Task<Income> GetIncome(ObjectId id)
        {
            return await _incomeRepository.GetIncome(id);
        }

        public async Task<List<Income>> GetIncomeByMonth(int month)
        {
            return await _incomeRepository.GetIncomeByMonth(month);
        }

        public async Task<List<Income>> GetIncomeByWeek(DateTime startDate)
        {
            return await _incomeRepository.GetIncomeByWeek(startDate);
        }

        public async Task UpdateIncome(Income income)
        {
            await _incomeRepository.UpdateIncome(income);
        }
    }
}
