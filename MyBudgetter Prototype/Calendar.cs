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
    }
}
