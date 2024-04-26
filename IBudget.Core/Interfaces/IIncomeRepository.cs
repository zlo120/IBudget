using IBudget.Core.Model;

namespace IBudget.Core.Interfaces
{
    public interface IIncomeRepository
    {
        Task<bool> AddIncome(Income income);
        Task<bool> DeleteIncome(Income income);
        Task<bool> UpdateIncome(Income income);
        Task<Income> GetIncome(int id);
        Task<List<Income>> GetIncomeByMonth(int month);
        Task<List<Income>> GetIncomeByWeek(DateTime startDate);
    }
}
