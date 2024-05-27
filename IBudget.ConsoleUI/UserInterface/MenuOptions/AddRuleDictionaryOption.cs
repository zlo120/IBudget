using IBudget.ConsoleUI.Utils;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using MyBudgetter_Prototype.Utils;

namespace IBudget.ConsoleUI.UserInterface.MenuOptions
{
    public class AddRuleDictionaryOption : MenuOption
    {
        private readonly ICSVParserService _CSVParserService;
        private readonly IUserExpenseDictionaryService _userExpenseDictionaryService;

        public AddRuleDictionaryOption(IIncomeService incomeService,
            IExpenseService expenseService, ISummaryService summaryService, ITagService tagService,
            ICSVParserService CSVParserService, IUserExpenseDictionaryService userExpenseDictionaryService)
        : base(incomeService, expenseService, summaryService, tagService)
        {
            _CSVParserService = CSVParserService;
            _userExpenseDictionaryService = userExpenseDictionaryService;
        }
        public async override void Execute()
        {
            Console.WriteLine(Label);
            var userId = 1; // replace with userId from config
            string fileLocation;
            UserInput.FilePrompt("Please drag and drop the csv file into this terminal then press enter: ", out fileLocation);

            var formattedFinancialCsv = await _CSVParserService.ParseCSV(fileLocation);
            var (untaggedRecords, taggedRecords) = await _CSVParserService.DistinguishTaggedAndUntagged(formattedFinancialCsv);
            var distinctUntaggedRecords = untaggedRecords.Select(record => record.Description).Distinct().ToArray();
            ConsoleStyler.PrintTitle("Here are your untagged expenses");
            foreach (var record in distinctUntaggedRecords)
                Console.WriteLine(record);

            Console.WriteLine("\nPlease add your rules now (non regular expressions as of yet). Your rule will be used in a contains() function.\nEnter nothing in rule to finish.");
            var rules = new List<RuleDictionary>();
            while(true)
            {
                var rule = UserInput.Prompt("Rule", true);
                if (rule is null || rule.Length == 0) break;

                var tags = UserInput.ContinuousPrompt($"Enter tag for {rule} rule", true);

                rules.Add(new RuleDictionary()
                {
                    rule = rule,
                    tags = tags
                });
            }

            if (rules.Count > 0)
            {
                foreach(var rule in rules)
                    await _userExpenseDictionaryService.AddRuleDictionary(1, rule);
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
