using IBudget.Core.DatabaseModel;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using IBudget.Core.RepositoryInterfaces;
using MongoDB.Bson;

namespace IBudget.Core.Services
{
    public class ExpenseTagService(IExpenseTagsRepository expenseTagsRepository) : IExpenseTagService
    {
        private readonly IExpenseTagsRepository _expenseTagsRepository = expenseTagsRepository;

        public async Task ClearCollection()
        {
            await _expenseTagsRepository.ClearCollection();
        }

        public async Task<ExpenseTag> CreateExpenseTag(ExpenseTag expenseTag)
        {
            return await _expenseTagsRepository.CreateExpenseTag(expenseTag);
        }

        public async Task DeleteExpenseTagById(ObjectId id)
        {
            await _expenseTagsRepository.DeleteExpenseTagById(id);
        }

        public async Task DeleteExpenseTagByTitle(string title)
        {
            await _expenseTagsRepository.DeleteExpenseTagByTitle(title);
        }

        public async Task<List<ExpenseTag>> GetAllExpenseTags()
        {
            return await _expenseTagsRepository.GetAllExpenseTags();
        }

        public async Task<ExpenseTag> GetExpenseTagById(ObjectId id)
        {
            return await _expenseTagsRepository.GetExpenseTagById(id);
        }

        public async Task<PaginatedResponse<ExpenseTag>> GetExpenseTagByPage(int pageNumber)
        {
            return await _expenseTagsRepository.GetExpenseTagByPage(pageNumber);
        }

        public async Task<ExpenseTag> GetExpenseTagByTitle(string title)
        {
            return await _expenseTagsRepository.GetExpenseTagByTitle(title);
        }

        public async Task<PaginatedResponse<ExpenseTag>> Search(string searchString, int pageNumber)
        {
            return await _expenseTagsRepository.Search(searchString, pageNumber);
        }

        public async Task<ExpenseTag> UpdateExpenseTag(ExpenseTag expenseTag)
        {
            return await _expenseTagsRepository.UpdateExpenseTag(expenseTag);
        }
    }
}
