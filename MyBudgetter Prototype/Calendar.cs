using MyBudgetter_Prototype.Model;

namespace MyBudgetter_Prototype
{
    public static class Calendar
    {
        public static Year InitiateCalendar()
        {
            int yearNum;
            Console.Write("Please enter the year: ");
            int.TryParse(Console.ReadLine(), out yearNum);

            var months = new List<Month>();
            var year = new Year(yearNum);

            for (int i = 1; i <= 12; i++)
            {
                months.Add(new Month(i, year));
            }

            year.Months = months;

            return year;
        }

        public static DateTime[] GetAllWeeks(int year, int month)
        {
            DateTime[] weeks = new DateTime[5];

            DateTime currentDate = new DateTime(year, month, 1);

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
                if (counter != 0 && currentDate.Month != month)
                {
                    break;
                }

                weeks[counter] = currentDate;

                counter++;
                currentDate = currentDate.AddDays(7);
            }

            return weeks;
        }
    }
}
