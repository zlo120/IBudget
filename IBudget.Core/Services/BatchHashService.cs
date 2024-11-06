using IBudget.Core.Interfaces;

namespace IBudget.Core.Services
{
    public class BatchHashService : IBatchHashService
    {
        private readonly IBatchHashRepository _batchHashRepository;

        public BatchHashService(IBatchHashRepository batchHashRepository)
        {
            _batchHashRepository = batchHashRepository;
        }

        public async Task<bool> HashExists(string hash)
        {
            return await _batchHashRepository.HashExists(hash);
        }

        public async Task InsertBatchHash(string hash)
        {
            await _batchHashRepository.InsertBatchHash(hash);
        }
    }
}
