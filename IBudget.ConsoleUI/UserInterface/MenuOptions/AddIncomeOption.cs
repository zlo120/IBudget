using IBudget.Core.Exceptions;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using IBudget.ConsoleUI.Utils;

namespace IBudget.ConsoleUI.UserInterface.MenuOptions
{
    public class AddIncomeOption(IIncomeService incomeService,
            IExpenseService expenseService, ISummaryService summaryService, ITagService tagService)
        : MenuOption(incomeService, expenseService, summaryService, tagService)
    {
        public override void Execute()
        {
            Console.WriteLine(Label);
            try
            {
                var date = UserInput.GetDate();
                var amount = UserInput.NumberPrompt("Amount: $");
                Console.WriteLine("Frequency");
                string[] frequncyOptions = { "Daily", "Weekly", "Fortnightly", "Monthly", "Yearly" };
                var frequency = UserInput.MultipleChoicePrompt(frequncyOptions, true);
                var source = UserInput.Prompt("Source (optional)", true);
                var tags = UserInput.GetTags();

                var income = new Income()
                {
                    Date = date,
                    Amount = amount,
                    Source = source,
                    Tags = tags
                };

                if (frequency > 0)
                {
                    income.Frequency = (Frequency)frequency;
                }

                _incomeService.AddIncome(income);

                Console.Clear();
                Console.WriteLine("Your income record has been successfully been inserted!");
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
