using MyBudgetter_Prototype.Data;
using MyBudgetter_Prototype.Model;

namespace MyBudgetter_Prototype.UserInterface
{
    public class MainUI
    {
        public MainUI()
        {
            var yearNum = 2023;

            while (true)
            {
                Console.Write("1. Add Income\n2. Add Expense\n3. Read week\n4. Read month\n5. Read year\n6. Update record\n7. Delete record\n8. Exit\nPlease select one: ");

                int decision;
                int.TryParse(Console.ReadLine(), out decision);

                DateTime? dateTime;
                string category;
                double amount;
                string frequencyInput;
                int frequency;
                string source;
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
                        frequencyInput = Console.ReadLine();
                        int.TryParse(frequencyInput, out frequency);

                        Console.Write("Source (optional): ");
                        source = Console.ReadLine();
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
                            income.Frequency = (Frequency)frequency;
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

                        Console.Write("Frequency:\n  1. Daily\n  2. Weekly\n  3. BiWeekly\n  4. Monthly\n  5. Yearly\n(Optional, you may leave this blank): ");
                        frequencyInput = Console.ReadLine();
                        int.TryParse(frequencyInput, out frequency);

                        // Notes
                        Console.Write("Notes (optional): ");
                        var notes = Console.ReadLine();

                        // Tags
                        var tags = GetTags();

                        var expense = new Expense()
                        {
                            Category = category,
                            Date = dateTime.Value,
                            Amount = amount,
                            Notes = notes,
                            Tags = tags
                        };

                        if (frequency > 0 && frequency < 6)
                        {
                            frequency = frequency - 1;
                            expense.Frequency = (Frequency)frequency;
                        }

                        Database.InsertExpense(expense);
                        break;

                    case 3:
                        dateTime = GetDate();
                        if (!dateTime.HasValue) break;

                        week = Database.GetWeek(Calendar.GetWeekRange(dateTime.Value));

                        Calendar.DisplayWeek(week);
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
                return userDate;
            }
            else
            {
                // Parsing failed
                Console.WriteLine("Invalid date format. Please enter a valid date.");
                return null;
            }
        }
        public static List<string> GetTags()
        {
            var tags = new List<string>();
            Console.WriteLine("Enter a tag associated with this expense (optional, enter nothing to continue): ");
            while (true)
            {

                Console.Write(" > ");
                var tag = Console.ReadLine();
                if (tag == "")
                {
                    break;
                }

                tags.Add(tag);
            }

            return tags;
        }
    }
}
