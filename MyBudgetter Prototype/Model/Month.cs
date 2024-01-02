using System.Globalization;

namespace MyBudgetter_Prototype.Model
{
    public class Month
    {
        public string MonthName { get; }
        public int MonthNum { get; set; }
        public List<Week> Weeks { get; private set; }
        public Month(int month)
        {
            MonthName = DateTimeFormatInfo.CurrentInfo.GetMonthName(month);
            MonthNum = month;
            Weeks = new List<Week>();

            GenerateAllWeeks();
        }

        private void GenerateAllWeeks()
        {
            DateTime currentDate = new DateTime(DateTime.Now.Year, MonthNum, 1);

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

                var label = $"{currentDate.ToShortDateString()} - {currentDate.AddDays(6).ToShortDateString()}";
                Weeks.Add(new Week(label));

                counter++;
                currentDate = currentDate.AddDays(7);
            }
        }

        public void PrintAllWeeks()
        {
            foreach (var week in Weeks)
            {
                Console.WriteLine(week.Label);
            }
        }
    }
}