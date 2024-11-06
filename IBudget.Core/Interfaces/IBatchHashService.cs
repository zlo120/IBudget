namespace IBudget.Core.Interfaces
{
    public interface IBatchHashService
    {
        Task<bool> HashExists(string hash);
        Task InsertBatchHash(string hash);
    }
}
