using IBudget.Core.DatabaseModel;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using IBudget.Core.RepositoryInterfaces;
using MongoDB.Bson;

namespace IBudget.Core.Services
{
    public class ExpenseRuleTagService(IExpenseRuleTagsRepository expenseRuleTagRepository) : IExpenseRuleTagService
    {
        private readonly IExpenseRuleTagsRepository _expenseRuleTagRepository = expenseRuleTagRepository;
        public async Task<ExpenseRuleTag> CreateExpenseRuleTag(ExpenseRuleTag expenseRuleTag)
        {
            return await _expenseRuleTagRepository.CreateExpenseRuleTag(expenseRuleTag);
        }

        public async Task DeleteExpenseRuleTagById(ObjectId id)
        {
            await _expenseRuleTagRepository.DeleteExpenseRuleTagById(id);
        }

        public async Task DeleteExpenseRuleTagByRule(string rule)
        {
            await _expenseRuleTagRepository.DeleteExpenseRuleTagByRule(rule);
        }

        public async Task<List<ExpenseRuleTag>> GetAllExpenseRuleTags()
        {
            return await _expenseRuleTagRepository.GetAllExpenseRuleTags();
        }

        public async Task<ExpenseRuleTag> GetExpenseRuleTagById(ObjectId id)
        {
            return await _expenseRuleTagRepository.GetExpenseRuleTagById(id);
        }

        public async Task<PaginatedResponse<ExpenseRuleTag>> GetExpenseRuleTagByPage(int pageNumber)
        {
            return await _expenseRuleTagRepository.GetExpenseRuleTagByPage(pageNumber);
        }

        public async Task<ExpenseRuleTag?> GetExpenseRuleTagByDescription(string description)
        {
            return await _expenseRuleTagRepository.GetExpenseRuleTagByDescription(description);
        }

        public async Task<ExpenseRuleTag> UpdateExpenseRuleTag(ExpenseRuleTag expenseRuleTag)
        {
            return await _expenseRuleTagRepository.UpdateExpenseRuleTag(expenseRuleTag);
        }
    }
}
