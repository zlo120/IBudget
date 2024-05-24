using IBudget.Core.Exceptions;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace IBudget.Infrastructure.Repositories
{
    public class UserExpenseDictionaryRepository : IUserExpenseDictionaryRepository
    {
        private readonly MongoDBContext _db;
        public UserExpenseDictionaryRepository(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("MongoDB"));
            var db = MongoDBContext.Create(client.GetDatabase("IBudget"));
            _db = db;
        }
        public async Task<bool> AddExpenseDictionary(UserExpenseDictionary expenseDictionary)
        {
            try
            {
                await _db.userExpenseDictionaries.AddAsync(expenseDictionary);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new MongoCRUDException(ex.Message);
            }

            return true;
        }

        public async Task<UserExpenseDictionary> GetExpenseDictionary(int userId)
        {
            return await _db.userExpenseDictionaries.FirstOrDefaultAsync(ed => ed.userId == userId);
        }

        public Task<bool> RemoveExpenseDictionary(string title)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateExpenseDictionary(List<ExpenseDictionary> expenseDictionaries, int userID)
        {
            var result = await _db.userExpenseDictionaries.FirstOrDefaultAsync(ed => ed.userId == userID);
            if (result is null) throw new RecordNotFoundException("A user with that ID does not exist"); // won't throw exception

            // find all the expenseDictionaries that do not exist in the userExpenseDictionary.ExpenseDictionaries
            var newExpenseDictionaries = expenseDictionaries.Except(result.ExpenseDictionaries).ToList();
            var existingExpenseDictionaries = expenseDictionaries.Intersect(result.ExpenseDictionaries);

            return true;
        }
    }
}