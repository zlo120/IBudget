using IBudget.Core.Exceptions;
using IBudget.Core.Interfaces;
using IBudget.Core.Utils;
using IBudget.ConsoleUI.Utils;

namespace IBudget.ConsoleUI.UserInterface.MenuOptions
{
    public class ReadWeekOption(IIncomeService incomeService,
            IExpenseService expenseService, ISummaryService summaryService, ITagService tagService)
        : MenuOption(incomeService, expenseService, summaryService, tagService)
    {
        public async override Task Execute()
        {
            Console.WriteLine(Label);

            try
            {
                var date = UserInput.GetDate();
                var week = _summaryService.ReadWeek(date).Result;
                Console.Clear();
                Calendar.DisplayWeek(week);
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