namespace IBudget.Core.Interfaces
{
    public interface IBatchHashRepository
    {
        Task<bool> HashExists(string hash);
        Task InsertBatchHash(string hash);
    }
}
