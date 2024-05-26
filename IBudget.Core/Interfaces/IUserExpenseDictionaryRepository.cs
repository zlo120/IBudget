using IBudget.Core.Model;

namespace IBudget.Core.Interfaces
{
    public interface IUserExpenseDictionaryRepository
    {
        Task<bool> AddExpenseDictionary(UserExpenseDictionary expenseDictionary);
        Task<UserExpenseDictionary> GetExpenseDictionary(int userId);
        Task<bool> UpdateExpenseDictionary(List<ExpenseDictionary> expenseDictionaries, int userID);
        Task<bool> RemoveExpenseDictionary(int userId, string title);
    }
}
