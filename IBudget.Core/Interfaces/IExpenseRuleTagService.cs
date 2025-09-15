using IBudget.Core.Model;

namespace IBudget.Core.Interfaces
{
    public interface IExpenseRuleTagService
    {
        Task<List<ExpenseRuleTag>> GetAllExpenseRuleTags();
    }
}
