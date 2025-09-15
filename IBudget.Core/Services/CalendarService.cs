using IBudget.Core.DTO;
using IBudget.Core.Interfaces;

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

        public async Task<MonthDTO> RetrieveMonthData(MonthDTO month)
        {
            month.AllIncome = _incomeService.GetIncomeByMonth(month.MonthNum).Result;
            month.AllExpenses = _expenseService.GetExpensesByMonth(month.MonthNum).Result;

            return month;
        }

        public async Task<WeekDTO> RetrieveWeekData(WeekDTO week)
        {
            week.Income = _incomeService.GetIncomeByWeek(week.Start).Result;
            week.Expenses = _expenseService.GetExpenseByWeek(week.Start).Result;

            return week;
        }
    }
}
