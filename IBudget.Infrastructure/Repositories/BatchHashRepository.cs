using IBudget.Core.Model;
using IBudget.Core.RepositoryInterfaces;

namespace IBudget.Infrastructure.Repositories
{
    public class BatchHashRepository : IBatchHashRepository
    {
        private readonly Context _context;

        public BatchHashRepository(Context context)
        {
            _context = context;
        }
        public async Task<bool> HashExists(string hash)
        {
            return _context.BatchHashes.Any(h => h.Hash == hash);
        }

        public async Task InsertBatchHash(string hash)
        {
            if (await HashExists(hash))
            {
                throw new Exception("Aborting insertion of batch hash, existing batch hash already exists!");
            }
            var batchHash = new BatchHash() { Hash = hash };
            _context.BatchHashes.Add(batchHash);
            await _context.SaveChangesAsync();
        }
    }
}
