using IBudget.Core.DatabaseModel;
using IBudget.Core.Model;
using MongoDB.Bson;

namespace IBudget.Core.Interfaces
{
    public interface IExpenseTagService
    {
        Task<ExpenseTag> CreateExpenseTag(ExpenseTag expenseTag);
        Task<ExpenseTag> GetExpenseTagByTitle(string title);
        Task<ExpenseTag> GetExpenseTagById(ObjectId id);
        Task<PaginatedResponse<ExpenseTag>> GetExpenseTagByPage(int pageNumber);
        Task<ExpenseTag> UpdateExpenseTag(ExpenseTag expenseTag);
        Task DeleteExpenseTagByTitle(string title);
        Task DeleteExpenseTagById(ObjectId id);
        Task<List<ExpenseTag>> GetAllExpenseTags();
        Task ClearCollection();
        Task<List<ExpenseTag>> Search(string searchString);
    }
}
