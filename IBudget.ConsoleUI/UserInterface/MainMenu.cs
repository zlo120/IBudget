using IBudget.ConsoleUI.UserInterface.MenuOptions;
using IBudget.ConsoleUI.Utils;
using IBudget.Core.Exceptions;
using IBudget.Core.Interfaces;
using IBudget.Spreadsheet.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace IBudget.ConsoleUI.UserInterface
{
    public class MainMenu : IMainMenu
    {
        private readonly ISpreadSheetGeneratorService _spreadsheetGenerator;
        private readonly IConfiguration _config;
        private readonly IUserDictionaryService _userDictionaryService;
        private readonly AddExpenseOption _addExpenseOption;
        private readonly AddIncomeOption _addIncomeOption;
        private readonly DeleteRecordOption _deleteRecordOption;
        private readonly ReadMonthOption _readMonthOption;
        private readonly ReadWeekOption _readWeekOption;
        private readonly UpdateRecordOption _updateRecordOption;
        private readonly ParseCSVOption _parseCSVOption;
        private readonly AddRuleDictionaryOption _addExpenseDictionaryOption;
        private readonly List<string> MENU_LABELS = ["Add income", "Add expense", "Read week", "Read month", "Update record", "Delete record", "Generate spreadsheet", "Parse CSV", "Add Dictionary Rule"];
        public MainMenu(IEnumerable<IMenuOption> menuOptions, ISpreadSheetGeneratorService spreadsheetGenerator, IConfiguration config, IUserDictionaryService userExpenseDictionaryService)
        {
            _spreadsheetGenerator = spreadsheetGenerator;
            _config = config;
            _userDictionaryService = userExpenseDictionaryService;

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
        public async Task MainMenuLoop()
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
                        await _addIncomeOption.Execute();
                        break;

                    // Add expense option
                    case 2:
                        _addExpenseOption.Label = MENU_LABELS[decision - 1];
                        await _addExpenseOption.Execute();
                        break;

                    // Read week
                    case 3:
                        _readWeekOption.Label = MENU_LABELS[decision - 1];
                        await _readWeekOption.Execute();
                        break;

                    // Read month
                    case 4:
                        _readMonthOption.Label = MENU_LABELS[decision - 1];
                        await _readMonthOption.Execute();
                        break;

                    // Update record
                    case 5:
                        _updateRecordOption.Label = MENU_LABELS[decision - 1];
                        await _updateRecordOption.Execute();
                        break;

                    // Delete record
                    case 6:
                        _deleteRecordOption.Label = MENU_LABELS[decision - 1];
                        await _deleteRecordOption.Execute();
                        break;

                    // Generate spreadsheet
                    case 7:
                        _spreadsheetGenerator.GenerateSpreadsheet();
                        break;

                    case 8:
                        _parseCSVOption.Label = MENU_LABELS[decision - 1];
                        await _parseCSVOption.Execute();
                        break;

                    case 9:
                        _addExpenseDictionaryOption.Label = MENU_LABELS[decision - 1];
                        await _addExpenseDictionaryOption.Execute();
                        break;

                    default:
                        break;
                }

                Console.Clear();
            }
        }
        private async Task SQLDbCheck()
        {
            Console.WriteLine("Verifying SQL db...");
            // Check to see if the db exists
            var dbType = _config["DBtype"];
            if (dbType == "SQLite")
            {
                var pathString = _config.GetConnectionString("SQLite"); ;
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
                        "\nPress any key to exit...");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
                else
                {
                    try
                    {
                        var fileInfo = new FileInfo(filePath);
                        var fileSizeInBytes = fileInfo.Length;
                        if (fileSizeInBytes < 30000)
                        {
                            Console.WriteLine("It does not seem the db file has a migration ran for it just yet. Please run migration and update the db\nPress any key to exit...");
                            Console.ReadKey();
                            Environment.Exit(0);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Environment.Exit(0);
                    }
                }
            }
            else if (dbType == "SQLServer")
            {
                // do nothing
            }
        }
        private async Task MongoDBStartupCheck()
        {
            Console.WriteLine("Verifying MongoDb...");
            try
            {
                await _userDictionaryService.AddUser(99999);
                await _userDictionaryService.AddUser(99999);
                await _userDictionaryService.RemoveUser(99999);
                await _userDictionaryService.RemoveUser(99999);
                Console.Write("You have not configured the MongoDB correctly, currently it allows duplicate UserExpenseDictionaries. " +
                    "\nPlease open mongosh and run the command: \ndb.userDictionaries.createIndex( { \"userId\": 1 }, { unique: true } )\n" +
                    "Press any key to exit...");
                Console.ReadKey();
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                await _userDictionaryService.RemoveUser(99999);
            }

            try
            {
                var userId = int.Parse(_config["MongoDbUserId"]);
                if (await _userDictionaryService.GetUser(userId) is null)
                    await _userDictionaryService.AddUser(userId);
            }
            catch (Exception ex) { }
        }
        public async Task Execute()
        {
            await SQLDbCheck();
            await MongoDBStartupCheck();
            await MainMenuLoop();
        }
    }
}