using IBudget.ConsoleUI.Utils;
using IBudget.Core.Exceptions;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;

namespace IBudget.ConsoleUI.UserInterface.MenuOptions
{
    public class UpdateRecordOption : MenuOption
    {
        private readonly IRecordUtility _recordUtils;
        public UpdateRecordOption(IIncomeService incomeService,
            IExpenseService expenseService, ISummaryService summaryService, ITagService tagService, IRecordUtility recordUtility)
        : base(incomeService, expenseService, summaryService, tagService)
        {
            _recordUtils = recordUtility;
        }
        public override async void Execute()
        {
            Console.WriteLine(Label);

            DataEntry result;
            try
            {
                result = await _recordUtils.FindRecord();
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
                Console.Clear();
                var options = new string[] { "Amount", "Notes" };
                var option = UserInput.MultipleChoicePrompt(options);


                Console.Clear();
                if (option == 1)
                {
                    var amount = UserInput.NumberPrompt("Amount: $");
                    PerformAmountUpdate(expense, amount);
                }
                else if (option == 2)
                {
                    var notes = UserInput.Prompt("Notes");
                    PerformNotesUpdate(expense, notes);
                }
            }
            else
            {
                var amount = UserInput.NumberPrompt("Amount: $");
                PerformAmountUpdate(result, amount);
            }

            Console.Clear();
            Console.WriteLine("Record updated successfully.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private void PerformNotesUpdate(Expense result, string notes)
        {
            if (result is null) throw new NullReferenceException("Record is null");

            result.Notes = notes;
            _expenseService.UpdateExpense(result);
        }

        private void PerformAmountUpdate(DataEntry record, double amount)
        {
            if (record is null) throw new NullReferenceException("Record is null");

            if (record is Expense expense)
            {
                expense.Amount = amount;
                _expenseService.UpdateExpense(expense);
            }
            else if (record is Income income)
            {
                income.Amount = amount;
                _incomeService.UpdateIncome(income);
            }
        }
    }
}
