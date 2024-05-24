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

        public async Task<UserExpenseDictionary> GetExpenseDictionary(int userId)
        {
            return await _expenseDictionaryRepository.GetExpenseDictionary(userId);
        }

        public async Task<bool> RemoveExpenseDictionary(string title)
        {
            return await _expenseDictionaryRepository.RemoveExpenseDictionary(title);
        }

        public async Task<bool> UpdateExpenseDictionary(List<ExpenseDictionary> expenseDictionaries, int userID)
        {
            try
            {
                return await _expenseDictionaryRepository.UpdateExpenseDictionary(expenseDictionaries, userID);
            }
            catch (RecordNotFoundException ex)
            {
                throw ex;
            }
        }
    }
}