using MyBudgetter_Prototype;

int yearNum;
Console.Write("Please enter the year: ");
int.TryParse(Console.ReadLine(), out yearNum);

var months = new List<Month>();
var year = new Year(yearNum);

for (int i = 1; i <= 12; i++)
{
    months.Add(new Month(i, year));
}

foreach(var month in months)
{
    Console.WriteLine($"{month.MonthName}: ");
    month.PrintAllWeeks();
    Console.WriteLine();
}