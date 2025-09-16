namespace IBudget.Core.Interfaces
{
    public interface IBatchHashService
    {
        string ComputeBatchHash(string input);
        Task<bool> DoesBatchHashExist(string hash);
    }
}
