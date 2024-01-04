using Core.Model;
using System.Globalization;

namespace Core
{
    public enum CalendarEnum
    {
        January,
        February,
        March,
        April,
        May,
        June,
        July,
        August,
        September,
        October,
        November,
        December
    }
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
                months.Add(new Month(i));
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
        public static DateTime[] GetWeekRange(DateTime date)
        {
            DateTime[] range = new DateTime[2];

            if (date.DayOfWeek != DayOfWeek.Sunday)
            {
                while (date.DayOfWeek != DayOfWeek.Sunday)
                {
                    date = date.AddDays(-1);
                }
            }

            range[0] = date;
            range[1] = date.AddDays(6);

            return range;
        }
        public static string GetWeekLabel(DateTime date)
        {
            if (date.DayOfWeek != DayOfWeek.Sunday)
            {
                while (date.DayOfWeek != DayOfWeek.Sunday)
                {
                    date = date.AddDays(-1);
                }
            }

            return $"{date.ToShortDateString()} - {date.AddDays(6).ToShortDateString()}";
        }
        public static void DisplayWeek(Week week)
        {
            Console.WriteLine();
            Console.WriteLine(week.Label);
            Console.WriteLine("Expenses:");

            foreach (var expense in week.Expenses)
            {
                Console.WriteLine($"ID: {expense.ID,-5} Date: {expense.Date.ToString("dd/MM/yyyy"),-15} Category: {expense.Category,-10} Amount: {expense.Amount.ToString("C", CultureInfo.GetCultureInfo("en-US")),-10}", "en-AU");
            }
            Console.WriteLine();

            Console.WriteLine("Income:");
            foreach (var income in week.Income)
            {
                Console.WriteLine($"ID: {income.ID,-5} Date: {income.Date.ToString("dd/MM/yyyy"),-15} Category: {income.Category,-10} Amount: {income.Amount.ToString("C", CultureInfo.GetCultureInfo("en-US")),-10}");
            }
            Console.WriteLine();
        }
    }
}
