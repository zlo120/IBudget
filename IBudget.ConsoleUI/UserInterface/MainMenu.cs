using IBudget.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using IBudget.ConsoleUI.Config;
using IBudget.ConsoleUI.UserInterface.MenuOptions;
using IBudget.ConsoleUI.Utils;
using IBudget.Spreadsheet;
using IBudget.Spreadsheet.Interfaces;

namespace IBudget.ConsoleUI.UserInterface
{
    public class MainMenu : IMainMenu
    {
        private readonly List<MenuConfigItem> _menuConfig;
        private readonly IGenerator _spreadsheetGenerator;
        private readonly AddExpenseOption _addExpenseOption;
        private readonly AddIncomeOption _addIncomeOption;
        private readonly DeleteRecordOption _deleteRecordOption;
        private readonly ReadMonthOption _readMonthOption;
        private readonly ReadWeekOption _readWeekOption;
        private readonly UpdateRecordOption _updateRecordOption;

        public MainMenu(IEnumerable<IMenuOption> menuOptions, IGenerator spreadsheetGenerator)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            _menuConfig = config.GetSection("MenuConfig").Get<List<MenuConfigItem>>();

            _spreadsheetGenerator = spreadsheetGenerator;

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
            }
        }

        public async void MainMenuLoop()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("MAIN MENU\n");

                var menuLabels = _menuConfig.Select(item => item.Label).ToList().ToArray();
                int decision;
                try
                {
                    decision = UserInput.MultipleChoicePrompt(menuLabels);
                }
                catch (InvalidInputException ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    continue;
                }

                Console.Clear();
                switch (decision)
                {
                    // Add income
                    case 1:
                        _addIncomeOption.Label = menuLabels[decision - 1];
                        _addIncomeOption.Execute();
                        break;

                    // Add expense option
                    case 2:
                        _addExpenseOption.Label = menuLabels[decision - 1];
                        _addExpenseOption.Execute();
                        break;

                    // Read week
                    case 3:
                        _readWeekOption.Label = menuLabels[decision - 1];
                        _readWeekOption.Execute();
                        break;

                    // Read month
                    case 4:
                        _readMonthOption.Label = menuLabels[decision - 1];
                        _readMonthOption.Execute();
                        break;

                    // Update record
                    case 5:
                        _updateRecordOption.Label = menuLabels[decision - 1];
                        _updateRecordOption.Execute();
                        break;

                    // Delete record
                    case 6:
                        _deleteRecordOption.Label = menuLabels[decision - 1];
                        _deleteRecordOption.Execute();
                        break;

                    // Generate spreadsheet
                    case 7:
                        _spreadsheetGenerator.GenerateSpreadsheet();
                        break;
                }

                Console.Clear();
            }
        }
    }
}