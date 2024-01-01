using MyBudgetter_Prototype.Model;
using MyBudgetter_Prototype.Data;

namespace MyBudgetter_Prototype.UserInterface
{
    public class MainUI
    {
        public MainUI()
        {
            var yearNum = 2023;           

            while(true)
            {
                Console.Write("1. Add Income\n2. Add Expense\n3. Read week\n4. Read month\n5. Read year\n6. Update record\n7. Delete record\n8. Exit\nPlease select one: ");

                int decision;
                int.TryParse(Console.ReadLine(), out decision);

                DateTime? dateTime;
                string category;
                double amount;
                Week week;
                Month month;
                Year year;

                switch (decision)
                {
                    // Add income
                    case 1:
                        dateTime = GetDate();
                        if (!dateTime.HasValue) break;

                        Console.Write("Category: ");
                        category = Console.ReadLine();

                        Console.Write("Amount: $");
                        double.TryParse(Console.ReadLine(), out amount);

                        Console.Write("Frequency:\n  1. Daily\n  2. Weekly\n  3. BiWeekly\n  4. Monthly\n  5. Yearly\n(Optional, you may leave this blank): ");
                        var frequencyInput = Console.ReadLine();
                        int frequency;
                        int.TryParse(frequencyInput, out frequency);

                        Console.Write("Source (optional): ");
                        var source = Console.ReadLine();
                        if (source == "")
                        {
                            source = null;
                        }

                        var income = new Income()
                        {
                            Category = category,
                            Date = dateTime.Value,
                            Amount = amount,
                            Source = source
                        };

                        if (frequency > 0 && frequency < 6)
                        {
                            frequency = frequency - 1;
                            income.Frequency = (Frequency) frequency;
                        }

                        Database.InsertIncome(income);

                        break;
                        
                    // Add expense
                    case 2:
                        dateTime = GetDate();
                        if (!dateTime.HasValue) break;

                        Console.Write("Category: ");
                        category = Console.ReadLine();

                        Console.Write("Amount: $");
                        double.TryParse(Console.ReadLine(), out amount);

                        var expense = new Expense()
                        {
                            Category = category,
                            Date = dateTime.Value,
                            Amount = amount
                        };

                        //Database.Insert(expense, "ExpenseRecord");

                        break;

                    case 3:
                        break;

                    case 4:
                        break;

                    case 5:
                        break;

                    case 6:
                        break;

                    case 7:
                        break;

                    case 8:
                        System.Environment.Exit(0);
                        break;

                    default:
                        break;
                }

                Console.WriteLine();
            }
        }
    
        public static DateTime? GetDate()
        {
            Console.Write("Please insert the date (format: dd/MM/yyyy): ");
            string userInput = Console.ReadLine();

            // Try to parse the user input into a DateTime
            if (DateTime.TryParseExact(userInput, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime userDate))
            {
                // Successfully parsed
                Console.WriteLine($"You entered: {userDate.ToShortDateString()}");
                return userDate;
            }
            else
            {
                // Parsing failed
                Console.WriteLine("Invalid date format. Please enter a valid date.");
                return null;
            }
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
            Console.WriteLine(week.Label);
            Console.WriteLine("Expenses:");

            foreach(var expense in week.Expenses)
            {
                Console.WriteLine($"Date: {expense.Date,-10} Category: {expense.Category,-10:C} Amount: {expense.Amount,-10:C}");
            }
            Console.WriteLine();

            Console.WriteLine("Income:");
            foreach (var income in week.Income)
            {
                Console.WriteLine($"Date: {income.Date,-10} Category: {income.Category,-10:C} Amount: {income.Amount,-10:C}");
            }
            Console.WriteLine();
        }
    }
}
