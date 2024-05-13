using IBudget.ConsoleUI.Utils;
using IBudget.Core.Interfaces;
using IBudget.CSVHandler;

namespace IBudget.ConsoleUI.UserInterface.MenuOptions
{
    public class ParseCSVOption(IIncomeService incomeService,
            IExpenseService expenseService, ISummaryService summaryService, ITagService tagService)
        : MenuOption(incomeService, expenseService, summaryService, tagService)
    {
        public override void Execute()
        {
            Console.WriteLine(Label);

            string fileLocation;
            UserInput.FilePrompt("Please drag and drop the csv file into this terminal then press enter: ", out fileLocation);

            var csvParser = new CSVParser(fileLocation);

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            return;
        }
    }
}
