using System.Globalization;
using System.Text.RegularExpressions;

namespace IBudget.Core.Utils
{
    public static class CsvFormatter
    {
        public static DateOnly FormatDate(string date)
        {
            DateTime dateTimeResult;
            if (!DateTime.TryParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeResult))
                throw new Exception("Date format is incorrect");

            return DateOnly.FromDateTime(dateTimeResult);
        }

        public static string FormatDescription(string description)
        {
            // Remove card info pattern
            var result = Regex.Replace(description, @" ?Card xx\d{4} Value Date: \d{2}/\d{2}/\d{4}", "");
            // Replace multiple spaces with a single space
            result = Regex.Replace(result, @" {2,}", " ");
            return result.Trim();
        }
    }
}