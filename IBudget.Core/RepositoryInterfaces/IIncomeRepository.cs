using IBudget.Core.Model;
using MongoDB.Bson;

namespace IBudget.Core.RepositoryInterfaces
{
    public interface IIncomeRepository
    {
        Task AddIncome(Income income);
        Task DeleteIncome(ObjectId id);
        Task UpdateIncome(Income income);
        Task<Income> GetIncome(ObjectId id);
        Task<List<Income>> GetIncomeByMonth(int month);
        Task<List<Income>> GetIncomeByWeek(DateTime startDate);
    }
}
