using IBudget.ConsoleUI.Utils;
using IBudget.Core.Exceptions;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using IBudget.Core.Services;
using System.Globalization;

namespace IBudget.ConsoleUI.UserInterface.MenuOptions
{
    public class ReadMonthOption : MenuOption
    {
        private readonly ICalendarService _calendarService;
        public ReadMonthOption(IIncomeService incomeService,
            IExpenseService expenseService, ISummaryService summaryService, ITagService tagService, ICalendarService calendarService)
        : base(incomeService, expenseService, summaryService, tagService)
        {
            _calendarService = calendarService;
        }
        public override void Execute()
        {
            Console.WriteLine(Label);

            try
            {
                // string array of all the months
                var months = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
                months = months.Where(m => !string.IsNullOrEmpty(m)).ToArray();
                // get the month from the user
                var userMonthInput = UserInput.MultipleChoicePrompt(months);
                var month = new Month(userMonthInput);
                month = _calendarService.RetrieveMonthData(month).Result;
                month.PopulateAllWeeks(_calendarService);

                Console.Clear();
                var border = new String('=', month.MonthName.Length * 3);
                Console.WriteLine();
                Console.WriteLine(border);
                Console.WriteLine($"Month: {month.MonthName}");
                Console.WriteLine(border);
                Console.WriteLine();

                UserInput.PrintTitle("Income");
                month.AllIncome.ForEach(i => Console.WriteLine(i.ToString()));

                Console.WriteLine();
                UserInput.PrintTitle("Expenses");
                month.AllExpenses.ForEach(e => Console.WriteLine(e.ToString()));

                Console.WriteLine();
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
