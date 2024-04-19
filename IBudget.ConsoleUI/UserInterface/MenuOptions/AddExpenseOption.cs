using Core.Exceptions;
using Core.Model;
using MyBudgetter_Prototype.Utils;

namespace MyBudgetter_Prototype.UserInterface.MenuOptions
{
    public class AddExpenseOption(MainMenu parent, string label, IServiceProvider serviceProvider)
        : MenuOption(parent, label, serviceProvider)
    {

        public override async void Execute()
        {
            Console.WriteLine(Label);
            try
            {
                var date = UserInput.GetDate();
                var amount = UserInput.NumberPrompt("Amount: $");
                Console.WriteLine("Frequency");
                string[] frequncyOptions = { "Daily", "Weekly", "Fortnightly", "Monthly", "Yearly" };
                var frequency = UserInput.MultipleChoicePrompt(frequncyOptions, true);
                var notes = UserInput.Prompt("Notes (optional)", true);
                var tags = UserInput.GetTags();

                var expense = new Expense()
                {
                    Date = date,
                    Amount = amount,
                    Tags = tags
                };

                if (!string.IsNullOrEmpty(notes))
                    expense.Notes = notes;

                if (frequency > 0)
                {
                    expense.Frequency = (Frequency)frequency;
                }

                _expenseService.AddExpense(expense);

                Console.Clear();
                Console.WriteLine("Your expense record has been successfully been inserted!");
                Console.Write("Press any key to continue...");
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
