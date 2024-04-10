using Core.Exceptions;
using Core.Model;
using MyBudgetter_Prototype.Utils;
using System.Globalization;

namespace MyBudgetter_Prototype.UserInterface.MenuOptions
{
    public class ReadMonthOption(MainMenu parent, string label, IServiceProvider serviceProvider)
        : MenuOption(parent, label, serviceProvider)
    {
        public override void Execute()
        {
            Console.WriteLine(Label);

            try
            {
                // string array of all the months
                var months = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
                months = months.Where(m => !string.IsNullOrEmpty(m)).ToArray();
                // get the month from the user
                var userMonthInput = UserInput.MultipleChoicePrompt(months);
                var month = new Month(userMonthInput, _serviceProvider);

                Console.Clear();
                var border = new String('=', month.MonthName.Length * 3);
                Console.WriteLine();
                Console.WriteLine(border);
                Console.WriteLine($"Month: {month.MonthName}");
                Console.WriteLine(border);
                Console.WriteLine();
                
                UserInput.PrintTitle("Income");
                month.AllIncome.ForEach(i => Console.WriteLine(i.ToString()));

                Console.WriteLine();
                UserInput.PrintTitle("Expenses");
                month.AllExpenses.ForEach(e => Console.WriteLine(e.ToString()));

                Console.WriteLine();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            catch (InvalidInputException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }   
        }
    }
}
