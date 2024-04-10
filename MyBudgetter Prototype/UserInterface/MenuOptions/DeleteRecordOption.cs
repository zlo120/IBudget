using Core.Exceptions;
using Core.Model;
using MyBudgetter_Prototype.Utils;

namespace MyBudgetter_Prototype.UserInterface
{
    public class DeleteRecordOption(MainMenu parent, string label, IServiceProvider serviceProvider)
        : MenuOption(parent, label, serviceProvider)
    {
        public override async void Execute()
        {
            Console.WriteLine(Label);
            var RecordUtils = new Record(_serviceProvider);
            DataEntry result;

            try
            {
                result = await RecordUtils.FindRecord();
            }
            catch (RecordNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            if (result is Expense expense)
            {
                _expenseService.DeleteExpense(expense);
            }
            else
            {
                _incomeService.DeleteIncome((Income) result);
            }

            Console.Clear();
            Console.WriteLine("Record deleted successfully.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}