using System.Globalization;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;

namespace IBudget.Core.DTO
{
    public class MonthDTO
    {
        private readonly int? _year;

        public string MonthName { get; }
        public int MonthNum { get; set; }
        public List<Income> AllIncome { get; set; } = new List<Income>();
        public List<Expense> AllExpenses { get; set; } = new List<Expense>();
        public List<DateTime[]> WeekRanges { get; private set; } = new List<DateTime[]>();
        public List<WeekDTO> Weeks { get; set; } = new List<WeekDTO>();
        public MonthDTO(int month)
        {
            MonthName = DateTimeFormatInfo.CurrentInfo.GetMonthName(month);
            MonthNum = month;
            GenerateAllWeeks();
        }

        public MonthDTO(int month, int year)
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

            if (currentDate.DayOfWeek != DayOfWeek.Monday
                && acceptableDays.Contains(currentDate.DayOfWeek.ToString()))
            {
                // count back until the nearest Sunday
                while (currentDate.DayOfWeek != DayOfWeek.Monday)
                {
                    currentDate = currentDate.AddDays(-1);
                }
            }
            else if (currentDate.DayOfWeek != DayOfWeek.Monday)
            {
                while (currentDate.DayOfWeek != DayOfWeek.Monday)
                {
                    currentDate = currentDate.AddDays(1);
                }
            }

            bool isFirstWeek = true;

            while (isFirstWeek || currentDate.Month == currentDate.AddDays(3).Month)
            {
                if (!isFirstWeek && currentDate.Month != MonthNum)
                {
                    break;
                }

                WeekRanges.Add([currentDate, currentDate.AddDays(6)]);

                isFirstWeek = false;
                currentDate = currentDate.AddDays(7);
            }

            foreach (var range in WeekRanges)
            {
                var week = new WeekDTO(range[0], range[1], Utils.Calendar.GetWeekLabel(range[0]));
                Weeks.Add(week);
            }
        }

        public void PopulateAllWeeks(ICalendarService calendarService)
        {
            for (int i = 0; i < Weeks.Count; i++)
            {
                Weeks[i] = calendarService.RetrieveWeekData(Weeks[i]).Result;
            }
        }
    }
}