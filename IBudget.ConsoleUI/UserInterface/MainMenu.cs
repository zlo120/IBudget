using IBudget.ConsoleUI.UserInterface.MenuOptions;
using IBudget.ConsoleUI.Utils;
using IBudget.Core.Exceptions;
using IBudget.Spreadsheet.Interfaces;

namespace IBudget.ConsoleUI.UserInterface
{
    public class MainMenu : IMainMenu
    {
        private readonly IGenerator _spreadsheetGenerator;
        private readonly AddExpenseOption _addExpenseOption;
        private readonly AddIncomeOption _addIncomeOption;
        private readonly DeleteRecordOption _deleteRecordOption;
        private readonly ReadMonthOption _readMonthOption;
        private readonly ReadWeekOption _readWeekOption;
        private readonly UpdateRecordOption _updateRecordOption;
        private readonly ParseCSVOption _parseCSVOption;

        private readonly string[] MENU_LABELS = ["Add income", "Add expense", "Read week", "Read month", "Update record", "Delete record", "Generate spreadsheet", "Parse CSV"];

        public MainMenu(IEnumerable<IMenuOption> menuOptions, IGenerator spreadsheetGenerator)
        {
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

                if (menuOption is ParseCSVOption)
                    _parseCSVOption = (ParseCSVOption)menuOption;
            }
        }

        public async void MainMenuLoop()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("MAIN MENU\n");

                int decision;
                try
                {
                    decision = UserInput.MultipleChoicePrompt(MENU_LABELS);
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

                    default:
                        break;
                }

                Console.Clear();
            }
        }
    }
}