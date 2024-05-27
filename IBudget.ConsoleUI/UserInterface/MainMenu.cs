using IBudget.ConsoleUI.UserInterface.MenuOptions;
using IBudget.ConsoleUI.Utils;
using IBudget.Core.Exceptions;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using IBudget.Spreadsheet.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace IBudget.ConsoleUI.UserInterface
{
    public class MainMenu : IMainMenu
    {
        private readonly IGenerator _spreadsheetGenerator;
        private readonly IConfiguration _config;
        private readonly IUserExpenseDictionaryService _userExpenseDictionaryService;
        private readonly AddExpenseOption _addExpenseOption;
        private readonly AddIncomeOption _addIncomeOption;
        private readonly DeleteRecordOption _deleteRecordOption;
        private readonly ReadMonthOption _readMonthOption;
        private readonly ReadWeekOption _readWeekOption;
        private readonly UpdateRecordOption _updateRecordOption;
        private readonly ParseCSVOption _parseCSVOption;
        private readonly AddRuleDictionaryOption _addExpenseDictionaryOption;

        

        private readonly List<string> MENU_LABELS = ["Add income", "Add expense", "Read week", "Read month", "Update record", "Delete record", "Generate spreadsheet", "Parse CSV", "Add Dictionary Rule"];

        public MainMenu(IEnumerable<IMenuOption> menuOptions, IGenerator spreadsheetGenerator, IConfiguration config, IUserExpenseDictionaryService userExpenseDictionaryService)
        {
            _spreadsheetGenerator = spreadsheetGenerator;
            _config = config;
            _userExpenseDictionaryService = userExpenseDictionaryService;

            foreach (var menuOption in menuOptions)
            {
                if (menuOption is AddExpenseOption)
                    _addExpenseOption = (AddExpenseOption)menuOption;

                if (menuOption is AddIncomeOption)
                    _addIncomeOption = (AddIncomeOption)menuOption;

                if (menuOption is DeleteRecordOption)
                    _deleteRecordOption = (DeleteRecordOption)menuOption;

                if (menuOption is ReadMonthOption)
                    _readMonthOption = (ReadMonthOption)menuOption;

                if (menuOption is ReadWeekOption)
                    _readWeekOption = (ReadWeekOption)menuOption;

                if (menuOption is UpdateRecordOption)
                    _updateRecordOption = (UpdateRecordOption)menuOption;

                if (menuOption is ParseCSVOption)
                    _parseCSVOption = (ParseCSVOption)menuOption;

                if (menuOption is AddRuleDictionaryOption)
                    _addExpenseDictionaryOption = (AddRuleDictionaryOption)menuOption;

            }
        }

        public async void MainMenuLoop()
        {
            MENU_LABELS.Add("Exit");
            while (true)
            {
                Console.Clear();
                Console.WriteLine("MAIN MENU\n");

                int decision;
                try
                {
                    decision = UserInput.MultipleChoicePrompt([.. MENU_LABELS]);
                }
                catch (InvalidInputException ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    continue;
                }

                Console.Clear();

                if (decision == MENU_LABELS.Count)
                {
                    ConsoleStyler.PrintTitle("GOOD BYE!");
                    Environment.Exit(0);
                }

                switch (decision)
                {
                    // Add income
                    case 1:
                        _addIncomeOption.Label = MENU_LABELS[decision - 1];
                        _addIncomeOption.Execute();
                        break;

                    // Add expense option
                    case 2:
                        _addExpenseOption.Label = MENU_LABELS[decision - 1];
                        _addExpenseOption.Execute();
                        break;

                    // Read week
                    case 3:
                        _readWeekOption.Label = MENU_LABELS[decision - 1];
                        _readWeekOption.Execute();
                        break;

                    // Read month
                    case 4:
                        _readMonthOption.Label = MENU_LABELS[decision - 1];
                        _readMonthOption.Execute();
                        break;

                    // Update record
                    case 5:
                        _updateRecordOption.Label = MENU_LABELS[decision - 1];
                        _updateRecordOption.Execute();
                        break;

                    // Delete record
                    case 6:
                        _deleteRecordOption.Label = MENU_LABELS[decision - 1];
                        _deleteRecordOption.Execute();
                        break;

                    // Generate spreadsheet
                    case 7:
                        _spreadsheetGenerator.GenerateSpreadsheet();
                        break;

                    case 8:
                        _parseCSVOption.Label = MENU_LABELS[decision - 1];
                        _parseCSVOption.Execute();
                        break;

                    case 9:
                        _addExpenseDictionaryOption.Label = MENU_LABELS[decision - 1];
                        _addExpenseDictionaryOption.Execute();
                        break;

                    default:
                        break;
                }

                Console.Clear();
            }
        }

        public async Task Execute()
        {
            SQLDbCheck();
            MongoDBStartupCheck();
            MainMenuLoop();
        }

        private void SQLDbCheck()
        {
            // Check to see if the db exists
            var dbType = _config["DBtype"];
            if (dbType == "SQLite")
            {
                var pathString = _config.GetConnectionString("SQLite");
                if (pathString == null)
                {
                    Console.WriteLine("No connection string exists, please edit the config file with the correct connection string");
                    Console.ReadLine();
                    Environment.Exit(0);
                }

                var filePath = pathString.Replace("Data Source=", "");
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("DB file does not exist, creating it now...");
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath.Replace("\\IBudget.db", ""));
                    }

                    var dbFile = File.Create(filePath);
                    dbFile.Close();

                    Console.Write("DB file created successfully. Please run migration to set up the db..." +
                        "\n\nTo generate migration execute this command: \n\tdotnet-ef migrations add MyMigration --context Context --project IBudget.Infrastructure --startup-project IBudget.ConsoleUI" +
                        "\nThen to execute migration run: \n\tdotnet-ef database update --context Context --project IBudget.Infrastructure  --startup-project IBudget.ConsoleUI" +
                        "\n\nPress any key to exit...");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }
            else if (dbType == "SQLServer")
            {
                // do nothing
            }
        }
        private async void MongoDBStartupCheck()
        {
            var userExpenseDictionary1 = new UserExpenseDictionary()
            {
                userId = 99999,
                RuleDictionary = new List<RuleDictionary>(),
                ExpenseDictionaries = new List<ExpenseDictionary>()
            };
            var userExpenseDictionary2 = new UserExpenseDictionary()
            {
                userId = 99999,
                RuleDictionary = new List<RuleDictionary>(),
                ExpenseDictionaries = new List<ExpenseDictionary>()
            };

            try
            {
                await _userExpenseDictionaryService.AddExpenseDictionary(userExpenseDictionary1);
                await _userExpenseDictionaryService.AddExpenseDictionary(userExpenseDictionary2);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}