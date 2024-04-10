using Core.Interfaces;
using Core.Model;

namespace Core.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;
        public ExpenseService(IExpenseRepository expenseRepository)
        {
            _expenseRepository = expenseRepository;
        }

        public async Task<bool> AddExpense(Expense expense)
        {
            return await _expenseRepository.AddExpense(expense);
        }

        public async Task<bool> DeleteExpense(Expense expense)
        {
            return await _expenseRepository.DeleteExpense(expense);
        }

        public async Task<Expense> GetExpense(int id)
        {
            return await _expenseRepository.GetExpense(id);
        }

        public async Task<List<Expense>> GetExpenseByWeek(DateTime startDate)
        {
            return await _expenseRepository.GetExpenseByWeek(startDate);
        }

        public async Task<List<Expense>> GetExpensesByMonth(int month)
        {
            return await _expenseRepository.GetExpensesByMonth(month);
        }

        public async Task<bool> UpdateExpense(Expense expense)
        {
            return await _expenseRepository.UpdateExpense(expense);
        }
    }
}
