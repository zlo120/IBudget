using IBudget.Core.Model;

namespace IBudget.Core.Interfaces
{
    public interface IUserExpenseDictionaryRepository
    {
        Task<bool> AddExpenseDictionary(UserExpenseDictionary expenseDictionary);
        Task<UserExpenseDictionary> GetExpenseDictionary(int userId);
        Task<bool> UpdateExpenseDictionary(List<ExpenseDictionary> expenseDictionaries, int userID);
        Task<bool> RemoveExpenseDictionary(int userId, string title);

        Task<bool> AddRuleDictionary(int userID, RuleDictionary ruleDictionary);
        Task<List<RuleDictionary>> GetRuleDictionaries(int userId);
        Task<bool> UpdateRuleDictionary(List<RuleDictionary> ruleDictionary, int userID);
        Task<bool> RemoveRuleDictionary(int userId, string rule);
    }
}