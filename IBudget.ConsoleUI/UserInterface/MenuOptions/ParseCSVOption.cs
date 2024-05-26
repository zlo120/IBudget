using IBudget.ConsoleUI.Utils;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;

namespace IBudget.ConsoleUI.UserInterface.MenuOptions
{
    public class ParseCSVOption : MenuOption
    {
        private readonly IUserExpenseDictionaryService _expenseDictionaryService;
        private readonly ICSVParserService _csvParserService;
        public ParseCSVOption(IIncomeService incomeService,
                              IExpenseService expenseService, 
                              ISummaryService summaryService, 
                              ITagService tagService,
                              IUserExpenseDictionaryService expenseDictionaryService,
                              ICSVParserService csvParserService)
        : base(incomeService, expenseService, summaryService, tagService)
        {
            _expenseDictionaryService = expenseDictionaryService;
            _csvParserService = csvParserService;
        }
        public override async void Execute()
        {
            Console.WriteLine(Label);
            var userId = 1;
            string fileLocation;
            UserInput.FilePrompt("Please drag and drop the csv file into this terminal then press enter: ", out fileLocation);
            try
            {
                _csvParserService.ParseCSV(fileLocation);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            return;
        }
    }
}