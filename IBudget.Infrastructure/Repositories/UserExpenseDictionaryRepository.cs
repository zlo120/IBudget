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
        private readonly MongoDBEFContext _db;
        public UserExpenseDictionaryRepository(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("MongoDB"));
            var db = MongoDBEFContext.Create(client.GetDatabase("IBudget"));
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

        public async Task<bool> AddRuleDictionary(int userId, RuleDictionary ruleDictionary)
        {
            var result = await GetExpenseDictionary(userId);
            bool userExisted = true;
            if (result is null)
            {
                userExisted = false;
                var user = new UserExpenseDictionary()
                {
                    userId = userId,
                    RuleDictionary = new List<RuleDictionary>()
                };
                result = user;
            }
            if (result.RuleDictionary is null) result.RuleDictionary = new List<RuleDictionary>();
            var ruleDictionaries = result.RuleDictionary.Select(rd => rd.rule).ToList();
            if (ruleDictionaries.Contains(ruleDictionary.rule)) // if the rule exists already, update it
            {
                return await UpdateRuleDictionary(new List<RuleDictionary> { ruleDictionary }, userId);
            }
            result.RuleDictionary.Add(ruleDictionary);

            try
            {
                if (!userExisted)
                {
                    await _db.userExpenseDictionaries.AddAsync(result);
                }
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

        public async Task<List<RuleDictionary>> GetRuleDictionaries(int userId)
        {
            var user = await GetExpenseDictionary(userId);
            return user.RuleDictionary;
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

        public async Task<bool> RemoveRuleDictionary(int userId, string rule)
        {
            var result = await GetExpenseDictionary(userId);
            if (result is null) throw new RecordNotFoundException("A user with that ID does not exist");

            var ruleDictionary = result.RuleDictionary.Where(ruleDictionary => ruleDictionary.rule == rule).FirstOrDefault();
            if (ruleDictionary is null) throw new RecordNotFoundException($"A rule dictionary with that rule does not exist for that user");

            result.RuleDictionary.Remove(ruleDictionary);

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

        public async Task<bool> RemoveUser(int userId)
        {
            var result = await GetExpenseDictionary(userId);
            if (result is null) throw new RecordNotFoundException("A user with that ID does not exist");

            try
            {
                _db.userExpenseDictionaries.Remove(result);
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
                foreach (var tag in updatedExpenseDictionary.tags)
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

        public async Task<bool> UpdateRuleDictionary(List<RuleDictionary> ruleDictionaries, int userID)
        {
            var result = await _db.userExpenseDictionaries.FirstOrDefaultAsync(rd => rd.userId == userID);
            if (result is null) throw new RecordNotFoundException("A user with that ID does not exist");

            var newRuleDictionaries = ruleDictionaries.Where(ed => !result.RuleDictionary.Contains(ed)).ToList();
            var duplicateDictionaries = result.RuleDictionary.Where(ruleDictionaries.Contains).ToList();

            foreach (var ruleDictionary in newRuleDictionaries)
                result.RuleDictionary.Add(ruleDictionary);

            foreach (var duplicateDictionary in duplicateDictionaries)
            {
                // this search will work for both existing and updated dictionary collections
                //  since comparing dictionaries is accomplished by comparing title strings.
                var existingRuleDictionary = result.RuleDictionary.Where(existingRuleDictionary =>
                    existingRuleDictionary.Equals(duplicateDictionary)).FirstOrDefault();

                var updatedRuleDictionary = ruleDictionaries.Where(updatedRuleDictionary =>
                    updatedRuleDictionary.Equals(duplicateDictionary)).FirstOrDefault();

                var combinedDictionaries = new List<ExpenseDictionary>();
                var distinctTags = new HashSet<string>(existingRuleDictionary.tags);
                foreach (var tag in updatedRuleDictionary.tags)
                    distinctTags.Add(tag);

                var indexForDuplicateDictionary = result.RuleDictionary.IndexOf(duplicateDictionary);
                result.RuleDictionary[indexForDuplicateDictionary].tags = distinctTags.ToArray();
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