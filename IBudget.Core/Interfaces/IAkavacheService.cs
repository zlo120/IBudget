using IBudget.Core.Model;

namespace IBudget.Core.Interfaces
{
    public interface IAkavacheService
    {
        Task<ExpenseDictionary> GetExpenseDictionary(string description);
        Task<RuleDictionary> GetRuleDictionary(string rule);
        Task SaveToExpenseDictionary(ExpenseDictionary expenseDictionary);
        Task SaveToRuleDictionary(RuleDictionary ruleDictionary);
        Task<List<string>> GetAllTags();
        Task SaveTag(string tag);
        Task<string> GetBatchHash(string hash);
        Task StoreBatchHash(string hash);
        Task<List<string>> GetAllKeys<T>();
    }
}
