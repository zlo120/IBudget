namespace IBudget.Core.RepositoryInterfaces
{
    public interface IBatchHashRepository
    {
        Task<bool> HashExists(string hash);
        Task InsertBatchHash(string hash);
    }
}
