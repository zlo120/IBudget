using IBudget.ConsoleUI.Utils;
using IBudget.Core.Exceptions;
using IBudget.Spreadsheet.Interfaces;

namespace IBudget.ConsoleUI.UserInterface
{
    public class MainMenu : IMainMenu
    {
        private readonly ISpreadSheetGeneratorService _spreadsheetGenerator;
        private readonly List<string> MENU_LABELS = ["Generate spreadsheet"];
        public MainMenu(ISpreadSheetGeneratorService spreadsheetGenerator)
        {
            _spreadsheetGenerator = spreadsheetGenerator;
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
                    // Generate spreadsheet
                    case 1:
                        _spreadsheetGenerator.GenerateSpreadsheet();
                        break;

                    default:
                        break;
                }

                Console.Clear();
            }
        }
        public async Task Execute()
        {
            await MainMenuLoop();
        }
    }
}