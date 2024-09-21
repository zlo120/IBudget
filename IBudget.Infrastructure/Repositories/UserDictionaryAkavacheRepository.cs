using IBudget.Core.Interfaces;
using IBudget.Core.Model;

namespace IBudget.Infrastructure.Repositories
{
    public class UserDictionaryAkavacheRepository : IUserDictionaryRepository
    {
        private readonly IAkavacheService _akavacheService;

        public UserDictionaryAkavacheRepository(IAkavacheService akavacheService)
        {
            _akavacheService = akavacheService;
        }
        public async Task<bool> AddExpenseDictionary(int userID, ExpenseDictionary expenseDictionary)
        {
            await _akavacheService.SaveToExpenseDictionary(expenseDictionary);
            return true;
        }

        public async Task<bool> AddRuleDictionary(int userId, RuleDictionary ruleDictionary)
        {
            await _akavacheService.SaveToRuleDictionary(ruleDictionary);
            return true;
        }

        public async Task CreateBatchHash(int userId, string hash)
        {
            await _akavacheService.StoreBatchHash(hash);
        }

        public async Task<List<ExpenseDictionary>?> GetExpenseDictionaries(int userId)
        {
            var allKeys = await _akavacheService.GetAllKeys<ExpenseDictionary>();
            if (allKeys is null) return null;
            var expenseDictionaries = new List<ExpenseDictionary>();
            foreach (var key in allKeys)
            {
                var expenseDictionary = await _akavacheService.GetExpenseDictionary(key);
                expenseDictionaries.Add(expenseDictionary);
            }
            return expenseDictionaries;
        }

        public async Task<List<RuleDictionary>?> GetRuleDictionaries(int userId)
        {
            var allKeys = await _akavacheService.GetAllKeys<RuleDictionary>();
            if (allKeys is null) return null;
            var ruleDictionaries = new List<RuleDictionary>();
            foreach (var key in allKeys)
            {
                var ruleDictionary = await _akavacheService.GetRuleDictionary(key);
                ruleDictionaries.Add(ruleDictionary);
            }
            return ruleDictionaries;
        }


        // Not implemented yet
        public async Task<bool> RemoveExpenseDictionary(int userId, string title)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveRuleDictionary(int userId, string rule)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateExpenseDictionary(ExpenseDictionary expenseDictionaries, int userID)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateRuleDictionary(RuleDictionary ruleDictionary, int userID)
        {
            throw new NotImplementedException();
        }

        // Never going to be implemented
        public Task<bool> AddUser(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<UserDictionary> GetUser(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveUser(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
