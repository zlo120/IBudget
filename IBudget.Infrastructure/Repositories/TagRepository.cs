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
        private readonly IConfiguration? _config;

        public TagRepository(Context context, IUserDictionaryService userDictionaryService, IConfiguration config)
        {
            _context = context;
            _userDictionaryService = userDictionaryService;
            _config = config;
        }

        public TagRepository(Context context, IUserDictionaryService userDictionaryService)
        {
            _context = context;
            _userDictionaryService = userDictionaryService;
        }


        public async Task CreateTag(Tag tag)
        {
            if (await GetTag(tag.Name) is not null) return;
            _context.Tags.Add(tag);
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
            int userId = int.Parse(_config?["MongoDbUserId"] ?? "-1");
            var userExpenseDictionary = await _userDictionaryService.GetExpenseDictionaries(userId);
            var userRulesDictionary = await _userDictionaryService.GetRuleDictionaries(userId);
            
            // title should be the same as the description (both should be formatted descriptions)
            var tags = userExpenseDictionary
                .Where(uED => uED.title.Equals(description, StringComparison.InvariantCultureIgnoreCase))? 
                .Select(uED => uED.tags)?
                .FirstOrDefault()?
                .ToList();

            if (tags is not null && tags.Count > 0) return tags;

            tags = userRulesDictionary
                .Where(uRD => description.Contains(uRD.rule, StringComparison.InvariantCultureIgnoreCase))?
                .Select(rD => rD.tags)?
                .FirstOrDefault()?
                .ToList();

            if (tags is not null && tags.Count > 0) return tags;
            throw new Exception("Could not find tag by description");
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