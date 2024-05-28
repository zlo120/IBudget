using IBudget.Core.Model;

namespace IBudget.Core.Interfaces
{
    public interface IUserDictionaryRepository
    {
        Task<bool> AddUser(int userId);
        Task<bool> RemoveUser(int userId);
        Task<UserDictionary> GetUser(int userId);
        Task<bool> AddExpenseDictionary(int userID, ExpenseDictionary expenseDictionary);
        Task<List<ExpenseDictionary>> GetExpenseDictionaries(int userId);
        Task<bool> UpdateExpenseDictionary(ExpenseDictionary expenseDictionaries, int userID);
        Task<bool> RemoveExpenseDictionary(int userId, string title);
        Task<bool> AddRuleDictionary(int userId, RuleDictionary ruleDictionary);
        Task<List<RuleDictionary>> GetRuleDictionaries(int userId);
        Task<bool> UpdateRuleDictionary(RuleDictionary ruleDictionary, int userID);
        Task<bool> RemoveRuleDictionary(int userId, string rule);
    }
}