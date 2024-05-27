using IBudget.Core.Exceptions;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;

namespace IBudget.Core.Services
{
    public class UserExpenseDictionaryService : IUserExpenseDictionaryService
    {
        private readonly IUserExpenseDictionaryRepository _expenseDictionaryRepository;

        public UserExpenseDictionaryService(IUserExpenseDictionaryRepository expenseDictionaryRepository) 
        { 
            _expenseDictionaryRepository = expenseDictionaryRepository;
        }
        public async Task<bool> AddExpenseDictionary(UserExpenseDictionary expenseDictionary)
        {
            return await _expenseDictionaryRepository.AddExpenseDictionary(expenseDictionary);
        }

        public async Task<bool> AddRuleDictionary(int userId, RuleDictionary ruleDictionary)
        {
            return await _expenseDictionaryRepository.AddRuleDictionary(userId, ruleDictionary);
        }

        public async Task<UserExpenseDictionary> GetExpenseDictionary(int userId)
        {
            return await _expenseDictionaryRepository.GetExpenseDictionary(userId);
        }

        public async Task<List<RuleDictionary>> GetRuleDictionaries(int userId)
        {
            return await _expenseDictionaryRepository.GetRuleDictionaries(userId);
        }

        public async Task<bool> RemoveExpenseDictionary(int userId, string title)
        {
            return await _expenseDictionaryRepository.RemoveExpenseDictionary(userId, title);
        }

        public async Task<bool> RemoveRuleDictionary(int userId, string rule)
        {
            return await _expenseDictionaryRepository.RemoveRuleDictionary(userId, rule);
        }

        public async Task<bool> UpdateExpenseDictionary(List<ExpenseDictionary> expenseDictionaries, int userID)
        {
            return await _expenseDictionaryRepository.UpdateExpenseDictionary(expenseDictionaries, userID);
        }

        public async Task<bool> UpdateRuleDictionary(List<RuleDictionary> ruleDictionary, int userID)
        {
            return await _expenseDictionaryRepository.UpdateRuleDictionary(ruleDictionary, userID);
        }
    }
}