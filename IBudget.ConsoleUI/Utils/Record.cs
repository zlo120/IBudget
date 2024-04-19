using Core.Exceptions;
using Core.Interfaces;
using Core.Model;
using System.Globalization;

namespace MyBudgetter_Prototype.Utils
{
    public class Record
    {
        private readonly ISummaryService _summaryService;
        private readonly ITagService? _tagService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IIncomeService _incomeService;
        private readonly IExpenseService _expenseService;

        public Record(IServiceProvider serviceProvider)
        {
            _incomeService = serviceProvider.GetService(typeof(IIncomeService)) as IIncomeService;
            _expenseService = serviceProvider.GetService(typeof(IExpenseService)) as IExpenseService;
            _summaryService = serviceProvider.GetService(typeof(ISummaryService)) as ISummaryService;
            _tagService = serviceProvider.GetService(typeof(ITagService)) as ITagService;
            _serviceProvider = serviceProvider;
        }

        public async Task<DataEntry> FindRecord()
        {
            string[] options = { "Income", "Expense" };
            var searchForIncome = true && UserInput.MultipleChoicePrompt(options) == 1;

            var months = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
            months = months.Where(m => !string.IsNullOrEmpty(m)).ToArray();

            Console.Clear();
            var monthDecision = UserInput.MultipleChoicePrompt(months);

            Console.Clear();
            Month result = new Month(monthDecision, _serviceProvider);
            UserInput.PrintTitle(result.MonthName);

            Console.WriteLine("NOTE: When selecting use the numbers starting from 1, not the IDs.");

            int recordDecision;
            if (searchForIncome)
            {
                if (result.AllIncome.Count == 0)
                {
                    throw new RecordNotFoundException("No income found for this month.");
                }

                // convert allIncome to string array
                var allIncomeStrings = result.AllIncome.Select(income => income.ToString()).ToArray();
                recordDecision = UserInput.MultipleChoicePrompt(allIncomeStrings);

                return result.AllIncome[recordDecision - 1];
            }
            else
            {
                if (result.AllExpenses.Count == 0)
                {
                    throw new RecordNotFoundException("No expenses found for this month.");
                }

                var allIncomeStrings = result.AllExpenses.Select(expense => expense.ToString()).ToArray();
                recordDecision = UserInput.MultipleChoicePrompt(allIncomeStrings);

                return result.AllExpenses[recordDecision - 1];
            }
        }
    }
}
