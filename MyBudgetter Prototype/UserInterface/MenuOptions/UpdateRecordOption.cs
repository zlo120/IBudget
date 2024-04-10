using Core.Exceptions;
using Core.Model;
using MyBudgetter_Prototype.Utils;

namespace MyBudgetter_Prototype.UserInterface.MenuOptions
{
    public class UpdateRecordOption(MainMenu parent, string label, IServiceProvider serviceProvider)
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

        private void PerformAmountUpdate(DataEntry record, int amount)
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
