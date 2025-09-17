using IBudget.Core.DTO;
using IBudget.Core.Interfaces;
using IBudget.Core.RepositoryInterfaces;
using IBudget.Core.Utils;

namespace IBudget.Core.Services
{
    public class SummaryService(IIncomeRepository incomeRepository, IExpenseRepository expenseRepository) : ISummaryService
    {
        private readonly IIncomeRepository _incomeRepository = incomeRepository;
        private readonly IExpenseRepository _expenseRepository = expenseRepository;

        public async Task<MonthDTO> ReadMonth(int month)
        {
            var incomes = await _incomeRepository.GetIncomeByMonth(month);
            var expenses = await _expenseRepository.GetExpensesByMonth(month);
            return new MonthDTO(month)
            {
                AllIncome = incomes,
                AllExpenses = expenses
            };
        }

        public async Task<WeekDTO> ReadWeek(DateTime date)
        {
            var weekRange = Calendar.GetWeekRange(date);
            var startOfWeek = weekRange[0];
            var endOfWeek = weekRange[1];
            var label = Calendar.GetWeekLabel(date);
            var incomes = await _incomeRepository.GetIncomeByWeek(startOfWeek);
            var expenses = await _expenseRepository.GetExpenseByWeek(startOfWeek);
            return new WeekDTO(startOfWeek, endOfWeek, label)
            {
                Income = incomes,
                Expenses = expenses
            };
        }
    }
}
