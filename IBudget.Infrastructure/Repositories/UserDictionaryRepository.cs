using IBudget.Core.Exceptions;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace IBudget.Infrastructure.Repositories
{
    public class UserDictionaryRepository : IUserDictionaryRepository
    {
        private readonly IMongoCollection<UserDictionary> _userDictionaries;
        public UserDictionaryRepository(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("MongoDB"));
            var database = client.GetDatabase("IBudget");
            _userDictionaries = database.GetCollection<UserDictionary>("userDictionaries");
        }
        public async Task<bool> AddExpenseDictionary(int userID, ExpenseDictionary expenseDictionary)
        {
            var user = await GetUser(userID);
            var expenseDictionaries = user.ExpenseDictionaries.Select(eD => eD.title).ToList();
            if (expenseDictionaries.Contains(expenseDictionary.title)) // if the title exists already, update it
            {
                return await UpdateExpenseDictionary(expenseDictionary, userID);
            }

            user.ExpenseDictionaries.Add(expenseDictionary);

            try
            {
                _userDictionaries.ReplaceOne(uD => uD.userId.Equals(userID), user);
            }
            catch (Exception ex)
            {
                throw new MongoCRUDException(ex.Message);
            }

            return true;
        }

        public async Task<bool> AddRuleDictionary(int userID, RuleDictionary ruleDictionary)
        {
            var user = await GetUser(userID);
            var ruleDictionaries = user.RuleDictionaries.Select(rD => rD.rule).ToList();
            if (ruleDictionaries.Contains(ruleDictionary.rule)) // if the title exists already, update it
            {
                return await UpdateRuleDictionary(ruleDictionary, userID);
            }

            user.RuleDictionaries.Add(ruleDictionary);

            try
            {
                _userDictionaries.ReplaceOne(uD => uD.userId.Equals(userID), user);
            }
            catch (Exception ex)
            {
                throw new MongoCRUDException(ex.Message);
            }

            return true;
        }

        public async Task<bool> AddUser(int userId)
        {
            var newUser = new UserDictionary()
            {
                userId = userId,
                RuleDictionaries = new List<RuleDictionary>(),
                ExpenseDictionaries = new List<ExpenseDictionary>(),
                BatchHashes = new List<string>()
            };

            try
            {
                _userDictionaries.InsertOne(newUser);
                return true;
            }
            catch (Exception ex)
            {
                throw new MongoCRUDException(ex.Message);
            }
        }

        public async Task CreateBatchHash(int userId, string hash)
        {
            var user = await GetUser(userId);
            user.BatchHashes.Add(hash);
            try
            {
                _userDictionaries.ReplaceOne(uD => uD.userId.Equals(userId), user);
            }
            catch (Exception ex)
            {
                throw new MongoCRUDException(ex.Message);
            }
        }

        public async Task<List<ExpenseDictionary>> GetExpenseDictionaries(int userId)
        {
            try
            {
                var user = await GetUser(userId);
                return user.ExpenseDictionaries;
            }
            catch(Exception ex)
            {
                throw new MongoCRUDException(ex.Message);
            }
        }

        public async Task<List<RuleDictionary>> GetRuleDictionaries(int userId)
        {
            try
            {
                var user = await GetUser(userId);
                return user.RuleDictionaries;
            }
            catch (Exception ex)
            {
                throw new MongoCRUDException(ex.Message);
            }
        }

        public async Task<UserDictionary> GetUser(int userId)
        {
            return await _userDictionaries.Find(uD => uD.userId.Equals(userId)).FirstOrDefaultAsync();
        }

        public async Task<bool> RemoveExpenseDictionary(int userId, string title)
        {
            var user = await GetUser(userId);
            var expenseDictionary = user.ExpenseDictionaries.Where(eD => eD.title.Equals(title)).FirstOrDefault();
            if (expenseDictionary is null) throw new Exception("Expense dictionary not found");
            user.ExpenseDictionaries.Remove(expenseDictionary);
            try
            {
                _userDictionaries.ReplaceOne(uD => uD.userId.Equals(userId), user);
            }
            catch (Exception ex)
            {
                throw new MongoCRUDException(ex.Message);
            }

            return true;
        }

        public async Task<bool> RemoveRuleDictionary(int userId, string rule)
        {
            var user = await GetUser(userId);
            var ruleDictionary = user.RuleDictionaries.Where(rD => rD.rule.Equals(rule)).FirstOrDefault();
            if (ruleDictionary is null) throw new Exception("Rule dictionary not found");
            user.RuleDictionaries.Remove(ruleDictionary);
            try
            {
                _userDictionaries.ReplaceOne(uD => uD.userId.Equals(userId), user);
            }
            catch (Exception ex)
            {
                throw new MongoCRUDException(ex.Message);
            }

            return true;
        }

        public async Task<bool> RemoveUser(int userId)
        {
            try
            {
                _userDictionaries.DeleteOne(uD => uD.userId.Equals(userId));
                return true;
            }
            catch (Exception ex)
            {
                throw new MongoCRUDException(ex.Message);
            }

        }

        public async Task<bool> UpdateExpenseDictionary(ExpenseDictionary updatedExpenseDictionary, int userID)
        {
            var user = await GetUser(userID);
            var expenseDictionary = user.ExpenseDictionaries.Where(eD => eD.title.Equals(updatedExpenseDictionary.title)).FirstOrDefault();
            if (expenseDictionary is null) throw new Exception("Expense dictionary not found");
            var indexOfExpenseDictionary = user.ExpenseDictionaries.IndexOf(expenseDictionary);
            user.ExpenseDictionaries[indexOfExpenseDictionary] = updatedExpenseDictionary;
            try
            {
                _userDictionaries.ReplaceOne(uD => uD.userId.Equals(userID), user);
                return true;
            }
            catch (Exception ex)
            {
                throw new MongoCRUDException(ex.Message);
            }
        }

        public async Task<bool> UpdateRuleDictionary(RuleDictionary updatedRuleDictionary, int userID)
        {
            var user = await GetUser(userID);
            var ruleDictionary = user.RuleDictionaries.Where(eD => eD.rule.Equals(updatedRuleDictionary.rule)).FirstOrDefault();
            if (ruleDictionary is null) throw new Exception("Rule dictionary not found");
            var indexOfRuleDictionary = user.RuleDictionaries.IndexOf(ruleDictionary);
            user.RuleDictionaries[indexOfRuleDictionary] = updatedRuleDictionary;
            try
            {
                _userDictionaries.ReplaceOne(uD => uD.userId.Equals(userID), user);
                return true;
            }
            catch (Exception ex)
            {
                throw new MongoCRUDException(ex.Message);
            }
        }
    }
}