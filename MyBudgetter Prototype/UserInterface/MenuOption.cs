using Core.Interfaces;
using Core.Model;

namespace MyBudgetter_Prototype.UserInterface
{
    public abstract class MenuOption : BaseModel
    {
        public MainMenu Parent { get; set; }
        public string Label { get; set; }

        protected readonly IIncomeService _incomeService;
        protected readonly IExpenseService _expenseService;
        protected readonly ISummaryService _summaryService;
        protected readonly ITagService _tagService;

        protected readonly IServiceProvider _serviceProvider;

        public MenuOption(MainMenu parent, string label, IServiceProvider serviceProvider)
        {
            Parent = parent;
            Label = label.ToUpper();

            _incomeService = serviceProvider.GetService(typeof(IIncomeService)) as IIncomeService;
            _expenseService = serviceProvider.GetService(typeof(IExpenseService)) as IExpenseService;
            _summaryService = serviceProvider.GetService(typeof(ISummaryService)) as ISummaryService;
            _tagService = serviceProvider.GetService(typeof(ITagService)) as ITagService;
            _serviceProvider = serviceProvider;
        }

        public abstract void Execute();
    }
}