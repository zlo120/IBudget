using IBudget.Core.Interfaces;
using IBudget.Core.Model;

namespace IBudget.Core.Services
{
    public class UserDictionaryService : IUserDictionaryService
    {
        private readonly IUserDictionaryRepository _userDictionaryRepository;

        public UserDictionaryService(IUserDictionaryRepository userDictionaryRepository)
        {
            _userDictionaryRepository = userDictionaryRepository;
        }

        public async Task<bool> AddExpenseDictionary(int userID, ExpenseDictionary expenseDictionary)
        {
            return await _userDictionaryRepository.AddExpenseDictionary(userID, expenseDictionary);
        }

        public async Task<bool> AddRuleDictionary(int userId, RuleDictionary ruleDictionary)
        {
            return await _userDictionaryRepository.AddRuleDictionary(userId, ruleDictionary);
        }

        public async Task<bool> AddUser(int userId)
        {
            return await _userDictionaryRepository.AddUser(userId);
        }

        public async Task CreateBatchHash(int userId, string hash)
        {
            await _userDictionaryRepository.CreateBatchHash(userId, hash);
        }

        public async Task<List<ExpenseDictionary>> GetExpenseDictionaries(int userId)
        {
            return await _userDictionaryRepository.GetExpenseDictionaries(userId);
        }

        public async Task<List<RuleDictionary>> GetRuleDictionaries(int userId)
        {
            return await _userDictionaryRepository.GetRuleDictionaries(userId);
        }

        public async Task<UserDictionary> GetUser(int userId)
        {
            return await _userDictionaryRepository.GetUser(userId);
        }

        public async Task<bool> RemoveExpenseDictionary(int userId, string title)
        {
            return await _userDictionaryRepository.RemoveExpenseDictionary(userId, title);
        }

        public async Task<bool> RemoveRuleDictionary(int userId, string rule)
        {
            return await _userDictionaryRepository.RemoveRuleDictionary(userId, rule);
        }

        public async Task<bool> RemoveUser(int userId)
        {
            return await _userDictionaryRepository.RemoveUser(userId);
        }

        public async Task<bool> UpdateExpenseDictionary(ExpenseDictionary expenseDictionaries, int userID)
        {
            return await _userDictionaryRepository.UpdateExpenseDictionary(expenseDictionaries, userID);
        }

        public async Task<bool> UpdateRuleDictionary(RuleDictionary ruleDictionary, int userID)
        {
            return await _userDictionaryRepository.UpdateRuleDictionary(ruleDictionary, userID);
        }
    }
}