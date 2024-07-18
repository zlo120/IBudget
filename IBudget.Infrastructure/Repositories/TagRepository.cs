using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using IBudget.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace IBudget.Infrastructure.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly Context _context;
        private readonly IUserDictionaryService _userDictionaryService;
        private readonly IConfiguration _config;

        public TagRepository(Context context, IUserDictionaryService userDictionaryService, IConfiguration config)
        {
            _context = context;
            _userDictionaryService = userDictionaryService;
            _config = config;
        }

        public async Task CreateTag(string name)
        {
            if (await GetTag(name) is not null) return;
            _context.Tags.Add(new Tag {  Name = name });
            _context.SaveChanges();
        }

        public async Task DeleteTag(string name)
        {
            var tag = await GetTag(name);
            if (tag is null) return;
            _context.Tags.Remove(tag);
            _context.SaveChanges();
        }

        public async Task<List<string>> FindTagByDescription(string description)
        {
            var userDictionary = await _userDictionaryService.GetExpenseDictionaries(int.Parse(_config["MongoDbUserId"]));
            var userRules = await _userDictionaryService.GetRuleDictionaries(int.Parse(_config["MongoDbUserId"]));
            var tags = userDictionary
                .Where(uD => uD.title.Equals(description, StringComparison.CurrentCultureIgnoreCase))?
                .Select(uD => uD.tags)?
                .FirstOrDefault()?
                .ToList();

            if (tags is not null && tags.Count > 0) return tags;

            tags = userRules
                .Where(uR => description.Contains(uR.rule, StringComparison.CurrentCultureIgnoreCase))?
                .Select(uD => uD.tags)?
                .FirstOrDefault()?
                .ToList();

            if (tags is not null && tags.Count > 0) return tags;
            return new List<string>();
        }

        public async Task<List<Tag>> GetAll()
        {
            return await _context.Tags.ToListAsync();
        }

        public async Task<Tag> GetTag(string name)
        {
            return await _context.Tags.FirstOrDefaultAsync(x => x.Name == name);
        }
    }
}