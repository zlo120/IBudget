using Core.Interfaces;

namespace Core.Model
{
    public class Week
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        private readonly IIncomeService _incomeService;
        private readonly IExpenseService _expenseService;

        public string Label { get; }
        public List<Expense> Expenses { get; set; }
        public List<Income> Income { get; set; }
        public Week(DateTime start, DateTime end, string label)
        {
            Label = label;

            Income = new List<Income>();
            Expenses = new List<Expense>();

            Start = start;
            End = end;
        }

        public Week(DateTime start, DateTime end, string label, IServiceProvider serviceProvider)
        {
            Label = label;

            Income = new List<Income>();
            Expenses = new List<Expense>();

            Start = start;
            End = end;

            _incomeService = serviceProvider.GetService(typeof(IIncomeService)) as IIncomeService;
            _expenseService = serviceProvider.GetService(typeof(IExpenseService)) as IExpenseService;

            Income = _incomeService.GetIncomeByWeek(start).Result;
            Expenses = _expenseService.GetExpenseByWeek(start).Result;
        }
    }
}