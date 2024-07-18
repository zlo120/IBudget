using IBudget.Core.Model;

namespace IBudget.Core.Interfaces
{
    public interface IUserDictionaryService
    {
        Task<bool> AddUser(int userId);
        Task<bool> RemoveUser(int userId);
        Task<UserDictionary> GetUser(int userId);
        Task<bool> AddExpenseDictionary(int userID, ExpenseDictionary expenseDictionary);
        Task<List<ExpenseDictionary>> GetExpenseDictionaries(int userId);
        Task<bool> UpdateExpenseDictionary(ExpenseDictionary updatedExpenseDictionaries, int userID);
        Task<bool> RemoveExpenseDictionary(int userId, string title);
        Task<bool> AddRuleDictionary(int userId, RuleDictionary ruleDictionary);
        Task<List<RuleDictionary>> GetRuleDictionaries(int userId);
        Task<bool> UpdateRuleDictionary(RuleDictionary updatedRuleDictionary, int userID);
        Task<bool> RemoveRuleDictionary(int userId, string rule);
        Task CreateBatchHash(int userId, string hash);
    }
}