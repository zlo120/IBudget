using IBudget.Core.Interfaces;
using IBudget.Core.Model;

namespace IBudget.Core.Services
{
    public class AkavacheService : IAkavacheService
    {
        private readonly IAkavacheRepository _akavacheRepository;

        public AkavacheService(IAkavacheRepository akavacheRepository)
        {
            _akavacheRepository = akavacheRepository;
        }

        public async Task<List<string>> GetAllTags()
        {
            return await _akavacheRepository.GetAllTags();
        }

        public async Task<string> GetBatchHash(string hash)
        {
            return await _akavacheRepository.GetBatchHash(hash);
        }

        public async Task<ExpenseDictionary> GetExpenseDictionary(string description)
        {
            return await _akavacheRepository.GetExpenseDictionary(description);
        }

        public async Task<RuleDictionary> GetRuleDictionary(string rule)
        {
            return await _akavacheRepository.GetRuleDictionary(rule);
        }

        public async Task SaveTag(string tag)
        {
            await _akavacheRepository.SaveTag(tag);
        }

        public async Task SaveToExpenseDictionary(ExpenseDictionary expenseDictionary)
        {
            await _akavacheRepository.SaveToExpenseDictionary(expenseDictionary);
        }

        public async Task SaveToRuleDictionary(RuleDictionary ruleDictionary)
        {
            await _akavacheRepository.SaveToRuleDictionary(ruleDictionary);
        }

        public async Task StoreBatchHash(string hash)
        {
            await _akavacheRepository.StoreBatchHash(hash);
        }

        public async Task<List<string>> GetAllKeys<T>()
        {
            return await _akavacheRepository.GetAllKeys<T>();
        }
    }
}
