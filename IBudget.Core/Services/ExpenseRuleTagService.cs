using IBudget.Core.DatabaseModel;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using MongoDB.Bson;

namespace IBudget.Core.Services
{
    public class ExpenseRuleTagService(IExpenseRuleTagService expenseRuleTagService) : IExpenseRuleTagService
    {
        private readonly IExpenseRuleTagService _expenseRuleTagService = expenseRuleTagService;
        public async Task<ExpenseRuleTag> CreateExpenseRuleTag(ExpenseRuleTag expenseRuleTag)
        {
            return await _expenseRuleTagService.CreateExpenseRuleTag(expenseRuleTag);
        }

        public async Task DeleteExpenseRuleTagById(ObjectId id)
        {
            await _expenseRuleTagService.DeleteExpenseRuleTagById(id);
        }

        public async Task DeleteExpenseRuleTagByRule(string rule)
        {
            await _expenseRuleTagService.DeleteExpenseRuleTagByRule(rule);
        }

        public async Task<List<ExpenseRuleTag>> GetAllExpenseRuleTags()
        {
            return await _expenseRuleTagService.GetAllExpenseRuleTags();
        }

        public async Task<ExpenseRuleTag> GetExpenseRuleTagById(ObjectId id)
        {
            return await _expenseRuleTagService.GetExpenseRuleTagById(id);
        }

        public async Task<PaginatedResponse<ExpenseRuleTag>> GetExpenseRuleTagByPage(int pageNumber)
        {
            return await _expenseRuleTagService.GetExpenseRuleTagByPage(pageNumber);
        }

        public async Task<ExpenseRuleTag> GetExpenseRuleTagByRule(string rule)
        {
            return await _expenseRuleTagService.GetExpenseRuleTagByRule(rule);
        }

        public async Task<ExpenseRuleTag> UpdateExpenseRuleTag(ExpenseRuleTag expenseRuleTag)
        {
            return await _expenseRuleTagService.UpdateExpenseRuleTag(expenseRuleTag);
        }
    }
}
