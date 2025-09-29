using IBudget.Core.Model;
using MongoDB.Bson;

namespace IBudget.Core.RepositoryInterfaces
{
    public interface IExpenseRepository
    {
        Task AddExpense(Expense expense);
        Task DeleteExpense(ObjectId id);
        Task UpdateExpense(Expense expense);
        Task<Expense> GetExpense(ObjectId id);
        Task<List<Expense>> GetExpensesByMonth(int month);
        Task<List<Expense>> GetExpenseByWeek(DateTime startDate);
        Task<bool> DoesBatchHashExist(string batchHash);
        Task ClearCollection();
    }
}
