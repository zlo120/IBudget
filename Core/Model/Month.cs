using System.Globalization;

namespace Core.Model
{
    public class Month
    {
        public string MonthName { get; }
        public int MonthNum { get; set; }
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

        private void GenerateAllWeeks()
        {
            DateTime currentDate = new DateTime(2023, MonthNum, 1);

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

                WeekRanges.Add(new DateTime[] { currentDate, currentDate.AddDays(6) });

                counter++;
                currentDate = currentDate.AddDays(7);
            }
        }
    }
}