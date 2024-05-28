using IBudget.Core.Exceptions;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace IBudget.Infrastructure.Repositories
{
    [Obsolete("This implementation is no longer in use", true)]
    public class UserDictionaryEFRepository : IUserDictionaryRepository
    {
        private readonly MongoDBEFContext _db;
        public UserDictionaryEFRepository(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("MongoDB"));
            var db = MongoDBEFContext.Create(client.GetDatabase("IBudget"));
            _db = db;
        }
        // User operations
        public Task<bool> AddUser(int userId)
        {
            throw new NotImplementedException();
        }
        public Task<UserDictionary> GetUser(int userId)
        {
            throw new NotImplementedException();
        }
        public Task<bool> UpdateUser(UserDictionary userExpenseDictionary)
        {
            throw new NotImplementedException();
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
        // Expense Dictionary operations
        public async Task<bool> AddExpenseDictionary(int userId, ExpenseDictionary expenseDictionary)
        {
            var result = await GetUser(userId);
            bool userExisted = true;
            if (result is null)
            {
                userExisted = false;
                var user = new UserDictionary()
                {
                    userId = userId,
                    ExpenseDictionaries = new List<ExpenseDictionary>()
                };
                result = user;
            }
            if (result.ExpenseDictionaries is null) result.ExpenseDictionaries = new List<ExpenseDictionary>();
            var expenseDictionaries = result.ExpenseDictionaries.Select(rd => rd.title).ToList();
            if (expenseDictionaries.Contains(expenseDictionary.title)) // if the title exists already, update it
            {
                return await UpdateExpenseDictionary(new List<ExpenseDictionary> { expenseDictionary }, userId);
            }
            result.ExpenseDictionaries.Add(expenseDictionary);

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
        public async Task<UserDictionary> GetExpenseDictionary(int userId)
        {
            return await _db.userExpenseDictionaries.FirstOrDefaultAsync(ed => ed.userId == userId);
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
        // Rule Dictinary operations
        public async Task<bool> AddRuleDictionary(int userId, RuleDictionary ruleDictionary)
        {
            var result = await GetExpenseDictionary(userId);
            bool userExisted = true;
            if (result is null)
            {
                userExisted = false;
                var user = new UserDictionary()
                {
                    userId = userId,
                    RuleDictionaries = new List<RuleDictionary>()
                };
                result = user;
            }
            if (result.RuleDictionaries is null) result.RuleDictionaries = new List<RuleDictionary>();
            var ruleDictionaries = result.RuleDictionaries.Select(rd => rd.rule).ToList();
            if (ruleDictionaries.Contains(ruleDictionary.rule)) // if the rule exists already, update it
            {
                return await UpdateRuleDictionary(new List<RuleDictionary> { ruleDictionary }, userId);
            }
            result.RuleDictionaries.Add(ruleDictionary);

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
        public async Task<List<RuleDictionary>> GetRuleDictionaries(int userId)
        {
            var user = await GetExpenseDictionary(userId);
            return user.RuleDictionaries;
        }
        public async Task<bool> UpdateRuleDictionary(List<RuleDictionary> ruleDictionaries, int userID)
        {
            var result = await _db.userExpenseDictionaries.FirstOrDefaultAsync(rd => rd.userId == userID);
            if (result is null) throw new RecordNotFoundException("A user with that ID does not exist");

            var newRuleDictionaries = ruleDictionaries.Where(ed => !result.RuleDictionaries.Contains(ed)).ToList();
            var duplicateDictionaries = result.RuleDictionaries.Where(ruleDictionaries.Contains).ToList();

            foreach (var ruleDictionary in newRuleDictionaries)
                result.RuleDictionaries.Add(ruleDictionary);

            foreach (var duplicateDictionary in duplicateDictionaries)
            {
                // this search will work for both existing and updated dictionary collections
                //  since comparing dictionaries is accomplished by comparing title strings.
                var existingRuleDictionary = result.RuleDictionaries.Where(existingRuleDictionary =>
                    existingRuleDictionary.Equals(duplicateDictionary)).FirstOrDefault();

                var updatedRuleDictionary = ruleDictionaries.Where(updatedRuleDictionary =>
                    updatedRuleDictionary.Equals(duplicateDictionary)).FirstOrDefault();

                var combinedDictionaries = new List<ExpenseDictionary>();
                var distinctTags = new HashSet<string>(existingRuleDictionary.tags);
                foreach (var tag in updatedRuleDictionary.tags)
                    distinctTags.Add(tag);

                var indexForDuplicateDictionary = result.RuleDictionaries.IndexOf(duplicateDictionary);
                result.RuleDictionaries[indexForDuplicateDictionary].tags = distinctTags.ToArray();
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
        public async Task<bool> RemoveRuleDictionary(int userId, string rule)
        {
            var result = await GetExpenseDictionary(userId);
            if (result is null) throw new RecordNotFoundException("A user with that ID does not exist");

            var ruleDictionary = result.RuleDictionaries.Where(ruleDictionary => ruleDictionary.rule == rule).FirstOrDefault();
            if (ruleDictionary is null) throw new RecordNotFoundException($"A rule dictionary with that rule does not exist for that user");

            result.RuleDictionaries.Remove(ruleDictionary);

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

        public Task<List<ExpenseDictionary>> GetExpenseDictionaries(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateExpenseDictionary(ExpenseDictionary expenseDictionaries, int userID)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateRuleDictionary(RuleDictionary ruleDictionary, int userID)
        {
            throw new NotImplementedException();
        }
    }
}