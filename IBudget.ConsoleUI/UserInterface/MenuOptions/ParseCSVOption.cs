using IBudget.ConsoleUI.Utils;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using Microsoft.Extensions.Configuration;

namespace IBudget.ConsoleUI.UserInterface.MenuOptions
{
    public class ParseCSVOption : MenuOption
    {
        private readonly IUserDictionaryService _expenseDictionaryService;
        private readonly ICSVParserService _csvParserService;
        private readonly int _userId;
        public ParseCSVOption(IIncomeService incomeService,
                              IExpenseService expenseService,
                              ISummaryService summaryService,
                              ITagService tagService,
                              IUserDictionaryService expenseDictionaryService,
                              ICSVParserService csvParserService,
                              IConfiguration config)
        : base(incomeService, expenseService, summaryService, tagService)
        {
            _expenseDictionaryService = expenseDictionaryService;
            _csvParserService = csvParserService;
            _userId = int.Parse(config["MongoDbUserId"]);
        }
        public async override Task Execute()
        {
            Console.WriteLine(Label);
            string fileLocation;
            UserInput.FilePrompt("Please drag and drop the csv file into this terminal then press enter: ", out fileLocation);
            try
            {
                var formattedFinancialCsv = await _csvParserService.ParseCSV(fileLocation);
                var untaggedRecords = await _csvParserService.FindUntagged(formattedFinancialCsv);
                
                if (untaggedRecords.Count > 0)
                {
                    var newlyTagged = TagRecords(untaggedRecords);
                    foreach (var taggedRecord in newlyTagged)
                    {
                        var expenseDictionary = new ExpenseTag()
                        {
                            title = taggedRecord.Key,
                            tags = taggedRecord.Value
                        };
                        await _expenseDictionaryService.AddExpenseDictionary(_userId, expenseDictionary);
                    }

                    foreach (var newlyTaggedRecord in untaggedRecords)
                    {
                        newlyTaggedRecord.Tags.AddRange(newlyTagged[newlyTaggedRecord.Description].ToList());
                    }
                }

                // everything now has a tag
                //foreach (var record in taggedRecords)
                //    AddExpenseIntoSQLDB(record);
                throw new NotImplementedException("Changes to removing tagged records from CsvParserService has broken this functionality.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            return;
        }
        private Dictionary<string, string[]> TagRecords(List<FormattedFinancialCSV> untaggedRecords)
        {
            var remainingRecords = untaggedRecords.Select(record => record.Description).Distinct().ToArray();
            var taggedRecords = new Dictionary<string, string[]>();
            var recordsToTag = new Stack<string>(remainingRecords);
            ConsoleStyler.PrintTitle($"Entering Tags - Type \"abort\" and then enter nothing to abort this.");
            while (recordsToTag.Count() > 0)
            {
                var record = recordsToTag.Pop();
                var tagsList = new List<string>();
                Console.WriteLine($"Please tag this record. You have {recordsToTag.Count} left.");
                var tags = UserInput.ContinuousPrompt($"Please enter tags for \"{record}\"");

                if (tags.Length <= 0)
                {
                    Console.WriteLine("Please provide at least one tag.");
                    continue;
                }

                if (tags.Contains("abort"))
                    break;

                taggedRecords.Add(record, tags);
            }

            return taggedRecords;
        }
        private async void AddExpenseIntoSQLDB(FormattedFinancialCSV formattedFinancialCSV)
        {
            var tags = new List<Tag>();
            foreach (var tag in formattedFinancialCSV.Tags)
            {
                tags.Add(new Tag()
                {
                    Name = tag
                });
            }
            var expense = new Expense()
            {
                Amount = formattedFinancialCSV.Amount,
                Date = formattedFinancialCSV.Date.ToDateTime(TimeOnly.Parse("00:00")),
                Notes = formattedFinancialCSV.Description,
                Tags = tags
            };

            await _expenseService.AddExpense(expense);
        }
    }
}