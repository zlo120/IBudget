using Core.Interfaces;
using Core.Model;

namespace Core.Services
{
    public class IncomeService : IIncomeService
    {
        private readonly IIncomeRepository _incomeRepository;
        public IncomeService(IIncomeRepository incomeRepository)
        {
            _incomeRepository = incomeRepository;
        }

        public async Task<bool> AddIncome(Income income)
        {
            for (int i = 0; i < income.Tags.Count; i++)
            {
                var tag = income.Tags[i];
                tag.Name = tag.Name.ToLower();
                income.Tags[i] = tag;
            }

            return await _incomeRepository.AddIncome(income);
        }

        public async Task<bool> DeleteIncome(Income income)
        {
            return await _incomeRepository.DeleteIncome(income);
        }

        public async Task<Income> GetIncome(int id)
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

        public async Task<bool> UpdateIncome(Income income)
        {
            return await _incomeRepository.UpdateIncome(income);
        }
    }
}
