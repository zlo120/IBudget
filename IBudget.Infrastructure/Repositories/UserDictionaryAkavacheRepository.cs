using Akavache;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using System.Reactive.Linq;
using System.Security.Policy;

namespace IBudget.Infrastructure.Repositories
{
    public class UserDictionaryAkavacheRepository : IUserDictionaryRepository
    {
        private const string BATCH_HASHES_KEY = "batch_hashes";
        private const string EXPENSES_KEY = "all_expenses";
        private const string RULES_KEY = "all_rules";
        private async Task SaveToDb<T>(string key, T obj) where T : class
        {
            await BlobCache.UserAccount.InsertObject(key, obj);
        }
        private async Task SaveListItemToDb<T>(string key, T obj) where T : class
        {
            List<T>? list = null;
            try
            {
                list = await BlobCache.UserAccount.GetObject<List<T>?>(key);
            }
            catch (KeyNotFoundException)
            {
                list = new List<T>();
            }
            list.Add(obj);
            await SaveToDb(key, list);
        }

        public async Task<bool> AddExpenseDictionary(int userID, ExpenseDictionary expenseDictionary)
        {
            await SaveToDb(expenseDictionary.title, expenseDictionary);
            await SaveListItemToDb(EXPENSES_KEY, expenseDictionary.title);
            return true;
        }

        public async Task<bool> AddRuleDictionary(int userId, RuleDictionary ruleDictionary)
        {
            await SaveToDb(ruleDictionary.rule, ruleDictionary);

            await SaveListItemToDb(RULES_KEY, ruleDictionary.rule);
            return true;
        }

        public async Task CreateBatchHash(int userId, string hash)
        {
            await SaveListItemToDb(BATCH_HASHES_KEY, hash);
        }

        public async Task<List<ExpenseDictionary>> GetExpenseDictionaries(int userId)
        {
            var expenseDictionaryNames = await BlobCache.UserAccount.GetObject<List<string>>(EXPENSES_KEY);
            var expenseDictionaries = new List<ExpenseDictionary>();
            foreach(var name in expenseDictionaryNames)
            {
                var expenseDictionary = await BlobCache.UserAccount.GetObject<ExpenseDictionary>(name);
                expenseDictionaries.Add(expenseDictionary);
            }
            return expenseDictionaries;
        }

        public async Task<List<RuleDictionary>> GetRuleDictionaries(int userId)
        {
            var ruleDictionaryNames = await BlobCache.UserAccount.GetObject<List<string>>(RULES_KEY);
            var ruleDictionaries = new List<RuleDictionary>();
            foreach (var rule in ruleDictionaryNames)
            {
                var ruleDictionary = await BlobCache.UserAccount.GetObject<RuleDictionary>(rule);
                ruleDictionaries.Add(ruleDictionary);
            }
            return ruleDictionaries;
        }

        public async Task<string?> GetBatchHash(string hash)
        {
            var hashes = await BlobCache.UserAccount.GetObject<List<string>>(BATCH_HASHES_KEY);
            return hashes.Find(h => h == hash);
        }

        public async Task InitialiseDB()
        {
            try
            {
                var expenseDictionaryNames = await BlobCache.UserAccount.GetObject<List<string>>(EXPENSES_KEY);
            }
            catch (KeyNotFoundException)
            {
                await SaveToDb(EXPENSES_KEY, new List<string>());
            }

            try
            {
                var expenseDictionaryNames = await BlobCache.UserAccount.GetObject<List<string>>(RULES_KEY);
            }
            catch (KeyNotFoundException)
            {
                await SaveToDb(RULES_KEY, new List<string>());
            }

            try
            {
                var expenseDictionaryNames = await BlobCache.UserAccount.GetObject<List<string>>(BATCH_HASHES_KEY);
            }
            catch (KeyNotFoundException)
            {
                await SaveToDb(BATCH_HASHES_KEY, new List<string>());
            }
        }

        // TO DO
        public Task<bool> RemoveExpenseDictionary(int userId, string title)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveRuleDictionary(int userId, string rule)
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

        // will not implement: not part of domain
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
