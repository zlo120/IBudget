using Core.Exceptions;
using Core.Utils;
using MyBudgetter_Prototype.Utils;

namespace MyBudgetter_Prototype.UserInterface.MenuOptions
{
    public class ReadWeekOption(MainMenu parent, string label, IServiceProvider serviceProvider)
        : MenuOption(parent, label, serviceProvider)
    {
        public override void Execute()
        {
            Console.WriteLine(Label);

            try
            {
                var date = UserInput.GetDate();
                var week = _summaryService.ReadWeek(date).Result;
                Console.Clear();
                Calendar.DisplayWeek(week);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            catch (InvalidInputException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }
    }
}