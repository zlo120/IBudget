using IBudget.Core.RepositoryInterfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using Tag = IBudget.Core.Model.Tag;

namespace IBudget.Infrastructure.Repositories
{
    public class TagsRepository(MongoDbContext context, IExpenseTagsRepository expenseTagsRepository, IExpenseRuleTagsRepository expenseRuleTagsRepository) : ITagsRepository
    {
        private readonly IMongoCollection<Tag> _tagsCollection = context.GetTagsCollection();
        private readonly IExpenseTagsRepository _expenseTagsRepository = expenseTagsRepository;
        private readonly IExpenseRuleTagsRepository _expenseRuleTagsRepository = expenseRuleTagsRepository;

        public async Task ClearCollection()
        {
            await _tagsCollection.DeleteManyAsync(_ => true);
        }

        public async Task CreateTag(Tag tag)
        {
            await _tagsCollection.InsertOneAsync(tag);
        }

        public async Task DeleteTagById(ObjectId id)
        {
            await _tagsCollection.DeleteOneAsync(e => e.Id == id);
        }

        public async Task DeleteTagByName(string name)
        {
            await _tagsCollection.DeleteOneAsync(e => e.Name == name);
        }

        public async Task<List<Tag>> FindTagsByDescription(string description)
        {
            var tags = new List<Tag>();
            var expenseRuleTags = await _expenseRuleTagsRepository.GetExpenseRuleTagByDescription(description);
            List<string> tagNames = [];
            if (expenseRuleTags is not null && expenseRuleTags.Tags.Count > 0)
            {
                tagNames = [.. expenseRuleTags.Tags.Distinct()];
                foreach (var tagName in tagNames)
                {
                    var tag = await GetTagByName(tagName);
                    if (tag != null) tags.Add(tag);
                }
                return tags;
            }

            tagNames = [.. (await _expenseTagsRepository.GetAllExpenseTags()).Where(e => e.Title.Contains(description, StringComparison.InvariantCultureIgnoreCase))
                .SelectMany(e => e.Tags)
                .Distinct()];
            if (tagNames.Count == 0) return [];
            foreach (var tagName in tagNames)
            {
                var tag = await GetTagByName(tagName);
                if (tag != null) tags.Add(tag);
            }
            return tags;
        }

        public async Task<List<Tag>> GetAll()
        {
            return await _tagsCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Tag> GetTagByName(string name)
        {
            return await _tagsCollection.Find(e => e.Name == name).FirstOrDefaultAsync();
        }

        public async Task UpdateTag(Tag tag)
        {
            await _tagsCollection.ReplaceOneAsync(e => e.Id == tag.Id, tag);
        }
    }
}