using IBudget.Core.Interfaces;
using IBudget.Core.Model;

namespace IBudget.ConsoleUI.UserInterface
{
    public abstract class MenuOption : BaseModel, IMenuOption
    {
        public string Label { get; set; }

        protected readonly IIncomeService _incomeService;
        protected readonly IExpenseService _expenseService;
        protected readonly ISummaryService _summaryService;
        protected readonly ITagService _tagService;

        public MenuOption(IIncomeService incomeService, 
            IExpenseService expenseService, ISummaryService summaryService, ITagService tagService)
        {
            _incomeService = incomeService;
            _expenseService = expenseService;
            _summaryService = summaryService;
            _tagService = tagService;
        }

        public abstract Task Execute();
    }
}