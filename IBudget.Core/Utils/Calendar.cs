using IBudget.Core.DTO;
using System.Globalization;

namespace IBudget.Core.Utils
{
    public enum CalendarEnum
    {
        January = 1,
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
        public static YearDTO InitiateCalendar()
        {
            int yearNum = DateTime.Now.Year;
            // get current year number

            var months = new List<MonthDTO>();
            var year = new YearDTO(yearNum);

            for (int i = 1; i <= 12; i++)
            {
                months.Add(new MonthDTO(i, yearNum));
            }

            year.Months = months;

            return year;
        }
        public static DateTime[] GetAllWeeks(int year, int month)
        {
            DateTime currentDate = new DateTime(year, month, 1);

            string[] acceptableDays = ["Tuesday", "Wednesday", "Thursday"];

            if (currentDate.DayOfWeek != DayOfWeek.Monday
                && acceptableDays.Contains(currentDate.DayOfWeek.ToString()))
            {
                // count back until the nearest Monday
                while (currentDate.DayOfWeek != DayOfWeek.Monday)
                {
                    currentDate = currentDate.AddDays(-1);
                }
            }
            else if (currentDate.DayOfWeek != DayOfWeek.Monday) // count forward to the nearest Monday
            {
                while (currentDate.DayOfWeek != DayOfWeek.Monday)
                {
                    currentDate = currentDate.AddDays(1);
                }
            }

            var weekStartDates = new List<DateTime>();
            bool isFirstWeek = true;

            while (isFirstWeek || currentDate.Month == currentDate.AddDays(3).Month)
            {
                if (!isFirstWeek && currentDate.Month != month)
                {
                    break;
                }

                weekStartDates.Add(currentDate);

                isFirstWeek = false;
                currentDate = currentDate.AddDays(7);
            }

            return [.. weekStartDates];
        }
        public static DateTime[] GetWeekRange(DateTime date)
        {
            DateTime[] range = new DateTime[2];

            if (date.DayOfWeek != DayOfWeek.Monday)
            {
                while (date.DayOfWeek != DayOfWeek.Monday)
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
            if (date.DayOfWeek != DayOfWeek.Monday)
            {
                while (date.DayOfWeek != DayOfWeek.Monday)
                {
                    date = date.AddDays(-1);
                }
            }
            // start variable date.ToShortDateString() but replace '/' with '-'
            var start = date.ToString("dd-MM-yyyy");
            var end = date.AddDays(6).ToString("dd-MM-yyyy");
            return $"{start} to {end}";
        }
        public static bool DatesAreInTheSameWeek(DateTime date1, DateTime date2)
        {
            var cal = DateTimeFormatInfo.CurrentInfo.Calendar;
            var d1 = date1.Date.AddDays(-1 * (int)cal.GetDayOfWeek(date1));
            var d2 = date2.Date.AddDays(-1 * (int)cal.GetDayOfWeek(date2));
            return d1 == d2;
        }
        public static DateTime ParseWeekStartFromWeekRange(string weekRange)
        {
            string[] dateParts = weekRange.Split(new[] { " to " }, StringSplitOptions.None);
            if (dateParts.Length != 2)
            {
                // throw string splitting exception
                throw new Exception("An error occurred when trying to split the worksheet week name string.");
            }
            var dateString = dateParts[0];
            string[] formats = { "d-MM-yyyy", "dd-MM-yyyy" };
            if (DateTime.TryParseExact(dateString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                return result;
            }
            else
            {
                // throw string parsing exception
                throw new Exception($"An error occurred when parse a d-MM-yyyy date string or dd-MM-yyyy date string, attempted to parse: {dateString}. Stacktrace: {Environment.StackTrace}");
            }
        }
    }
}