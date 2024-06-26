﻿using IBudget.Core.Model;

namespace IBudget.Core.Interfaces
{
    public interface IExpenseRepository
    {
        Task<bool> AddExpense(Expense expense);
        Task<bool> DeleteExpense(Expense expense);
        Task<bool> UpdateExpense(Expense expense);
        Task<Expense> GetExpense(int id);
        Task<List<Expense>> GetExpensesByMonth(int month);
        Task<List<Expense>> GetExpenseByWeek(DateTime startDate);
    }
}
