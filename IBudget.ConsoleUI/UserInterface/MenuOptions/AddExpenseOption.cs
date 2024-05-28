using IBudget.Core.Exceptions;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using IBudget.ConsoleUI.Utils;

namespace IBudget.ConsoleUI.UserInterface.MenuOptions
{
    public class AddExpenseOption(IIncomeService incomeService,
            IExpenseService expenseService, ISummaryService summaryService, ITagService tagService)
        : MenuOption(incomeService, expenseService, summaryService, tagService)
    {

        public async override Task Execute()
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
