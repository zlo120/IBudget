using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using IBudget.Core.RepositoryInterfaces;
using MongoDB.Bson;

namespace IBudget.Core.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;
        public ExpenseService(IExpenseRepository expenseRepository)
        {
            _expenseRepository = expenseRepository;
        }

        public async Task AddExpense(Expense expense)
        {
            for (int i = 0; i < expense.Tags.Count; i++)
            {
                var tag = expense.Tags[i];
                tag.Name = tag.Name.ToLower();
                expense.Tags[i] = tag;
            }

            await _expenseRepository.AddExpense(expense);
        }

        public async Task DeleteExpense(ObjectId id)
        {
            await _expenseRepository.DeleteExpense(id);
        }

        public async Task<Expense> GetExpense(ObjectId id)
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

        public async Task UpdateExpense(Expense expense)
        {
            await _expenseRepository.UpdateExpense(expense);
        }
    }
}
