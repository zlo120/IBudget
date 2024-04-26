using IBudget.Core.Interfaces;
using IBudget.Core.Model;

namespace IBudget.Core.Services
{
    public class CalendarService : ICalendarService
    {
        private readonly IIncomeService _incomeService;
        private readonly IExpenseService _expenseService;

        public CalendarService(IIncomeService incomeService, IExpenseService expenseService)
        {
            _incomeService = incomeService;
            _expenseService = expenseService;
        }

        public async Task<Month> RetrieveMonthData(Month month)
        {
            month.AllIncome = _incomeService.GetIncomeByMonth(month.MonthNum).Result;
            month.AllExpenses = _expenseService.GetExpensesByMonth(month.MonthNum).Result;

            return month;
        }

        public async Task<Week> RetrieveWeekData(Week week)
        {
            week.Income = _incomeService.GetIncomeByWeek(week.Start).Result;
            week.Expenses = _expenseService.GetExpenseByWeek(week.Start).Result;

            return week;
        }
    }
}
