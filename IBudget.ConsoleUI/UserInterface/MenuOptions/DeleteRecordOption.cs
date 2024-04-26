using IBudget.ConsoleUI.Utils;
using IBudget.Core.Exceptions;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;

namespace IBudget.ConsoleUI.UserInterface.MenuOptions
{
    public class DeleteRecordOption : MenuOption
    {
        private readonly IRecordUtility _recordUtility;
        public DeleteRecordOption(IIncomeService incomeService,
            IExpenseService expenseService, ISummaryService summaryService, ITagService tagService, IRecordUtility recordUtility)
        : base(incomeService, expenseService, summaryService, tagService)
        {
            _recordUtility = recordUtility;
        }
        public override async void Execute()
        {
            Console.WriteLine(Label);
            DataEntry result;

            try
            {
                result = await _recordUtility.FindRecord();
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
                _incomeService.DeleteIncome((Income)result);
            }

            Console.Clear();
            Console.WriteLine("Record deleted successfully.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}