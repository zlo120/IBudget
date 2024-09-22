using Akavache;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using System.Globalization;
using System.Reactive.Linq;

namespace IBudget.Infrastructure.Repositories
{
    public class AkavacheRepository : IAkavacheRepository
    {
        private const string TAGS_KEY = "tags";
        private const string BATCH_HASHES_KEY = "batch_hashes";
        public async Task<List<string>> GetAllTags()
        {
            try
            {
                return await BlobCache.UserAccount.GetObject<List<string>>(TAGS_KEY);
            }
            catch (KeyNotFoundException)
            {
                return new List<string>();
            }
        }
        public async Task SaveTag(string tag)
        {
            var allTags = await GetAllTags();
            allTags.Add(tag);
            await BlobCache.UserAccount.InsertObject(TAGS_KEY, allTags);
        }
        private async Task<List<string>> GetAllBatchHashes()
        {
            try
            {
                return await BlobCache.UserAccount.GetObject<List<string>>(BATCH_HASHES_KEY) as List<string>;
            }
            catch (KeyNotFoundException)
            {
                return new List<string>();
            }
        }
        public async Task<string?> GetBatchHash(string hash)
        {
            var allHashes = await GetAllBatchHashes();
            return allHashes.Where(h => h == hash).First();
        }
        public async Task StoreBatchHash(string hash)
        {
            var allBatchHashes = await GetAllBatchHashes();
            allBatchHashes.Add(hash);
            await BlobCache.UserAccount.InsertObject(BATCH_HASHES_KEY, allBatchHashes);
        }
        public async Task<ExpenseDictionary> GetExpenseDictionary(string description)
        {
            return await GetItemFromDb<ExpenseDictionary>(description);
        }
        public async Task<RuleDictionary> GetRuleDictionary(string rule)
        {
            return await GetItemFromDb<RuleDictionary>(rule);
        }
        public async Task SaveToExpenseDictionary(ExpenseDictionary expenseDictionary)
        {
            await SaveToDb(expenseDictionary, expenseDictionary.title);
        }
        public async Task SaveToRuleDictionary(RuleDictionary ruleDictionary)
        {
            await SaveToDb(ruleDictionary, ruleDictionary.rule);
        }
        public async Task<List<string>?> GetAllKeys<T>()
        {
            try
            {
                return await BlobCache.UserAccount.GetObject<List<string>>(typeof(T).Name);
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }
        public async Task InsertFinance(FormattedFinancialCSV finance)
        {
            var monthString = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(finance.Date.Month);
            var year = finance.Date.Year;
            var key = $"{monthString}-{year}";
            var existingFinanceList = await GetItemFromDb<List<FormattedFinancialCSV>>(key) ?? new List<FormattedFinancialCSV>();
            existingFinanceList.Add(finance);
            await SaveToDb(existingFinanceList, key);
        }
        public async Task<List<FormattedFinancialCSV>> GetMonthsFinancialData(string key)
        {
            return await GetItemFromDb<List<FormattedFinancialCSV>>(key);
        }
        private async Task SaveKey<T>(string key)
        {
            var listOfKeys = await GetItemFromDb<List<string>>(typeof(T).Name);
            listOfKeys.Add(key);
            await BlobCache.UserAccount.InsertObject(typeof(T).Name.Replace("?", ""), listOfKeys); // saving each type as the key, and the paired value is the key in another table
        }
        private async Task SaveToDb<T>(T  obj, string key) where T : class
        {
            await BlobCache.UserAccount.InsertObject(key, obj);
            await SaveKey<T>(key);
        }
        private async Task<T> GetItemFromDb<T>(string key) where T : class
        {
            try
            {
                var result = await BlobCache.UserAccount.GetObject<T>(key); 
                if (result == null)
                {
                    var initValue = Activator.CreateInstance(typeof(T));
                    await SaveToDb(initValue!, key);
                    return await BlobCache.UserAccount.GetObject<T>(key);
                }
                return result;
            }
            catch(KeyNotFoundException)
            {
                var initValue = Activator.CreateInstance(typeof(T));
                await SaveToDb(initValue!, key);
                return await BlobCache.UserAccount.GetObject<T>(key);
            }
        }
    }
}