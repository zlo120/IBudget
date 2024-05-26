using IBudget.Core.Exceptions;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.ComponentModel.DataAnnotations;

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

        public async Task<bool> RemoveExpenseDictionary(int userId, string title)
        {
            var result = await GetExpenseDictionary(userId);
            if (result is null) throw new RecordNotFoundException("A user with that ID does not exist");

            var expenseDictionary = result.ExpenseDictionaries.Where(expenseDictionary => expenseDictionary.title == title).FirstOrDefault();
            if (expenseDictionary is null) throw new RecordNotFoundException($"An expense dictionary with that title does not exist for that user");

            result.ExpenseDictionaries.Remove(expenseDictionary);

            try
            {
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new MongoCRUDException(ex.Message);
            }
        }

        public async Task<bool> UpdateExpenseDictionary(List<ExpenseDictionary> expenseDictionaries, int userID)
        {
            var result = await _db.userExpenseDictionaries.FirstOrDefaultAsync(ed => ed.userId == userID);
            if (result is null) throw new RecordNotFoundException("A user with that ID does not exist");
            var newExpenseDictionaries = expenseDictionaries.Where(ed => !result.ExpenseDictionaries.Contains(ed)).ToList();
            var duplicateDictionaries = result.ExpenseDictionaries.Where(expenseDictionaries.Contains).ToList();

            foreach (var expenseDictionary in newExpenseDictionaries)
                result.ExpenseDictionaries.Add(expenseDictionary);

            foreach (var duplicateDictionary in duplicateDictionaries)
            {
                // this search will work for both existing and updated dictionary collections
                //  since comparing dictionaries is accomplished by comparing title strings.
                var existingExpenseDictionary = result.ExpenseDictionaries.Where(existingExpenseDictionary => 
                    existingExpenseDictionary.Equals(duplicateDictionary)).FirstOrDefault();

                var updatedExpenseDictionary = expenseDictionaries.Where(updatedExpenseDictionary =>
                    updatedExpenseDictionary.Equals(duplicateDictionary)).FirstOrDefault();

                var combinedDictionaries = new List<ExpenseDictionary>();
                var distinctTags = new HashSet<string>(existingExpenseDictionary.tags);
                foreach(var tag in updatedExpenseDictionary.tags)
                    distinctTags.Add(tag);

                var indexForDuplicateDictionary = result.ExpenseDictionaries.IndexOf(duplicateDictionary);
                result.ExpenseDictionaries[indexForDuplicateDictionary].tags = distinctTags.ToArray();
            }

            try
            {
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new MongoCRUDException(ex.Message);
            }

            return true;
        }
    }
}