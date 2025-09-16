using IBudget.Core.DatabaseModel;
using IBudget.Core.Model;
using MongoDB.Bson;

namespace IBudget.Core.Interfaces
{
    public interface IExpenseRuleTagService
    {
        Task<ExpenseRuleTag> CreateExpenseRuleTag(ExpenseRuleTag expenseRuleTag);
        Task<ExpenseRuleTag> GetExpenseRuleTagByRule(string rule);
        Task<ExpenseRuleTag> GetExpenseRuleTagById(ObjectId id);
        Task<PaginatedResponse<ExpenseRuleTag>> GetExpenseRuleTagByPage(int pageNumber);
        Task<ExpenseRuleTag> UpdateExpenseRuleTag(ExpenseRuleTag expenseRuleTag);
        Task DeleteExpenseRuleTagByRule(string rule);
        Task DeleteExpenseRuleTagById(ObjectId id);
        Task<List<ExpenseRuleTag>> GetAllExpenseRuleTags();
    }
}
