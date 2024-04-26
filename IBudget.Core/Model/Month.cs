using IBudget.Core.Interfaces;
using System.Globalization;

namespace IBudget.Core.Model
{
    public class Month
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IIncomeService _incomeService;
        private readonly IExpenseService _expenseService;

        private readonly int? _year;

        public string MonthName { get; }
        public int MonthNum { get; set; }
        public List<Income> AllIncome { get; set; } = new List<Income>();   
        public List<Expense> AllExpenses { get; set; } = new List<Expense>();   
        public List<DateTime[]> WeekRanges { get; private set; } = new List<DateTime[]>();
        public List<Week> Weeks { get; set; } = new List<Week>();
        public Month(int month)
        {
            MonthName = DateTimeFormatInfo.CurrentInfo.GetMonthName(month);
            MonthNum = month;
            GenerateAllWeeks();
        }

        public Month(int month, int year)
        {
            MonthName = DateTimeFormatInfo.CurrentInfo.GetMonthName(month);
            MonthNum = month;
            _year = year;
            GenerateAllWeeks();
        }

        private void GenerateAllWeeks()
        {
            DateTime currentDate;
            if (_year is not null) currentDate = new DateTime((int)_year, MonthNum, 1);
            else currentDate = new DateTime(DateTime.Today.Year, MonthNum, 1);

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
                var week = new Week(range[0], range[1], Utils.Calendar.GetWeekLabel(range[0]));
                Weeks.Add(week);
            }
        }
    
        public void PopulateAllWeeks(ICalendarService calendarService)
        {
            for(int i = 0; i < Weeks.Count; i++)
            {
                Weeks[i] = calendarService.RetrieveWeekData(Weeks[i]).Result;
            }
        }
    }
}