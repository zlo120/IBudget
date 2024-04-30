using IBudget.Core.Exceptions;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using MyBudgetter_Prototype.Utils;
using System.Globalization;

namespace IBudget.ConsoleUI.Utils
{
    public class RecordUtility : IRecordUtility
    {
        private readonly ICalendarService _calendarService;

        public RecordUtility(ICalendarService calendarService)
        {
            _calendarService = calendarService;
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
            Month result = new Month(monthDecision);
            result = await _calendarService.RetrieveMonthData(result);
            result.PopulateAllWeeks(_calendarService);
            ConsoleStyler.PrintTitle(result.MonthName);

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