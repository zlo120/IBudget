using IBudget.Core.Model;

namespace IBudget.Core.Interfaces
{
    public interface IExpenseTagService
    {
        Task<List<ExpenseTag>> GetAllExpenseTags();

    }
}
