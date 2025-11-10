using IBudget.Core.DatabaseModel;
using IBudget.Core.Model;
using MongoDB.Bson;

namespace IBudget.Core.RepositoryInterfaces
{
    public interface IExpenseRuleTagsRepository
    {
        Task<ExpenseRuleTag> CreateExpenseRuleTag(ExpenseRuleTag expenseRuleTag);
        Task<ExpenseRuleTag?> GetExpenseRuleTagByDescription(string rule);
        Task<ExpenseRuleTag> GetExpenseRuleTagById(ObjectId id);
        Task<PaginatedResponse<ExpenseRuleTag>> GetExpenseRuleTagByPage(int pageNumber);
        Task<ExpenseRuleTag> UpdateExpenseRuleTag(ExpenseRuleTag expenseRuleTag);
        Task DeleteExpenseRuleTagByRule(string description);
        Task DeleteExpenseRuleTagById(ObjectId id);
        Task<List<ExpenseRuleTag>> GetAllExpenseRuleTags();
        Task ClearCollection();
        Task<PaginatedResponse<ExpenseRuleTag>> Search(string searchString, int pageNumber);
    }
}
