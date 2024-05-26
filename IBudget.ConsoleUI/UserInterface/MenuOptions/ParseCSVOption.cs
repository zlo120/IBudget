using IBudget.ConsoleUI.Utils;
using IBudget.Core.Exceptions;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;

namespace IBudget.ConsoleUI.UserInterface.MenuOptions
{
    public class ParseCSVOption : MenuOption
    {
        private readonly IUserExpenseDictionaryService _expenseDictionaryService;
        public ParseCSVOption(IIncomeService incomeService,
            IExpenseService expenseService, ISummaryService summaryService, ITagService tagService,
            IUserExpenseDictionaryService expenseDictionaryService)
        : base(incomeService, expenseService, summaryService, tagService)
        {
            _expenseDictionaryService = expenseDictionaryService;
        }
        public override async void Execute()
        {
            Console.WriteLine(Label);

            // THIS IS JUST TESTING THE EXPENSE DICTIONARY REPOSITORY

            int userId = 1;
            var eds = new List<ExpenseDictionary>();
            while(true)
            {
                Console.WriteLine("New Expense Dictionary:");
                Console.Write("Title: ");
                var edTitle = Console.ReadLine();
                if (edTitle is null || edTitle.Equals("")) break;
                var ed = new ExpenseDictionary()
                {
                    title = edTitle,
                    tags = []
                };
                var tags = new List<string>();
                Console.WriteLine("New tag: ");
                while (true)
                {
                    Console.Write("Title: ");
                    var tagTitle = Console.ReadLine();
                    if (tagTitle is null || tagTitle.Equals("")) break;
                    tags.Add(tagTitle);
                }
                ed.tags = tags.ToArray();
                eds.Add(ed);
            }

            try
            {
                //await _expenseDictionaryService.AddExpenseDictionary(expenseDictionary);
                await _expenseDictionaryService.UpdateExpenseDictionary(eds, userId);
            }
            catch (RecordNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }

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