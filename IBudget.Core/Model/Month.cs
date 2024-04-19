using Core.Interfaces;
using System.Globalization;

namespace Core.Model
{
    public class Month
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IIncomeService _incomeService;
        private readonly IExpenseService _expenseService;

        public string MonthName { get; }
        public int MonthNum { get; set; }
        public List<Income> AllIncome { get; set; }
        public List<Expense> AllExpenses { get; set; }
        public List<DateTime[]> WeekRanges { get; private set; }
        public List<Week> Weeks { get; set; }
        public Month(int month)
        {
            MonthName = DateTimeFormatInfo.CurrentInfo.GetMonthName(month);
            MonthNum = month;
            WeekRanges = new List<DateTime[]>();
            Weeks = new List<Week>();

            GenerateAllWeeks();
        }
        public Month(int month, IServiceProvider serviceProvider)
        {
            MonthName = DateTimeFormatInfo.CurrentInfo.GetMonthName(month);
            MonthNum = month;
            WeekRanges = new List<DateTime[]>();
            Weeks = new List<Week>();

            _serviceProvider = serviceProvider;

            _incomeService = serviceProvider.GetService(typeof(IIncomeService)) as IIncomeService;
            _expenseService = serviceProvider.GetService(typeof(IExpenseService)) as IExpenseService;

            AllIncome = _incomeService.GetIncomeByMonth(month).Result;
            AllExpenses = _expenseService.GetExpensesByMonth(month).Result;

            GenerateAllWeeks();
        }
        
        private void GenerateAllWeeks()
        {
            DateTime currentDate = new DateTime(DateTime.Today.Year, MonthNum, 1);

            string[] acceptableDays = ["Monday", "Tuesday", "Wednesday"];

            var firstWeek = true;

            if (currentDate.DayOfWeek != DayOfWeek.Sunday
                && acceptableDays.Contains(currentDate.DayOfWeek.ToString()))
            {
                // count back until the nearest Sunday
                while (currentDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    currentDate = currentDate.AddDays(-1);
                }
            }
            else if (currentDate.DayOfWeek != DayOfWeek.Sunday)
            {
                while (currentDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    currentDate = currentDate.AddDays(1);
                }
            }

            int counter = 0;
            while (counter == 0 || currentDate.Month == currentDate.AddDays(3).Month)
            {
                if (counter != 0 && currentDate.Month != MonthNum)
                {
                    break;
                }

                WeekRanges.Add([currentDate, currentDate.AddDays(6)]);

                counter++;
                currentDate = currentDate.AddDays(7);
            }

            foreach (var range in WeekRanges)
            {
                if (_serviceProvider is not null)
                {
                    var week = new Week(range[0], range[1], Utils.Calendar.GetWeekLabel(range[0]), _serviceProvider);
                    Weeks.Add(week);
                }
                else
                {
                    var week = new Week(range[0], range[1], Utils.Calendar.GetWeekLabel(range[0]));
                    Weeks.Add(week);
                }
            }
        }
    }
}