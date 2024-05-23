using IBudget.ConsoleUI.Utils;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using IBudget.Core.Services;
using IBudget.CSVHandler;

namespace IBudget.ConsoleUI.UserInterface.MenuOptions
{
    public class ParseCSVOption : MenuOption
    {
        private readonly IExpenseDictionaryService _expenseDictionaryService;
        public ParseCSVOption(IIncomeService incomeService,
            IExpenseService expenseService, ISummaryService summaryService, ITagService tagService,
            IExpenseDictionaryService expenseDictionaryService)
        : base(incomeService, expenseService, summaryService, tagService)
        {
            _expenseDictionaryService = expenseDictionaryService;
        }
        public override void Execute()
        {
            Console.WriteLine(Label);

            // THIS IS JUST TESTING THE EXPENSE DICTIONARY REPOSITORY

            int userId = (int) UserInput.NumberPrompt("Please enter ID");

            string tag1 = UserInput.Prompt("Please enter tag1");
            string tag2 = UserInput.Prompt("Please enter tag2");

            var exDict1 = new ExpenseDictionary()
            {
                title = "exDict1",
                tags = new[] { tag1 }
            };

            var exDict2 = new ExpenseDictionary()
            {
                title = "exDict2",
                tags = new[] { tag2 }
            };

            var expenseDictionary = new UserExpenseDictionary()
            {
                userId = userId,
                ExpenseDictionaries = new List<ExpenseDictionary>() { exDict1, exDict2 }
            };

            _expenseDictionaryService.UpdateExpenseDictionary(new List<ExpenseDictionary>() { exDict1, exDict2 }, userId);

            // THE REAL START OF THIS MENU OPTION

            //string fileLocation;

            //UserInput.FilePrompt("Please drag and drop the csv file into this terminal then press enter: ", out fileLocation);

            //var csvParser = new CSVParser(fileLocation);



            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            return;
        }
    }
}