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
            await _tagsCollection.DeleteManyAsync(t => t.Name != "ignored");
        }

        public async Task CreateTag(Tag tag)
        {
            try
            {
                await _tagsCollection.InsertOneAsync(tag);
            }
            catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
            {
                // Silently ignore duplicate key errors
            }
            catch (MongoWriteException)
            {
                // Silently ignore any other MongoDB write errors during insert (per user request)
            }
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
                    var tag = await GetOrCreateTagByName(tagName);
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
                var tag = await GetOrCreateTagByName(tagName);
                if (tag != null) tags.Add(tag);
            }

            return tags;
        }

        public async Task<List<Tag>> GetAll()
        {
            return await _tagsCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Tag> GetOrCreateTagByName(string name)
        {
            try
            {
                var tag = await _tagsCollection.Find(e => e.Name == name).FirstOrDefaultAsync();
                if (tag is null)
                {
                    tag = new Tag { Name = name, IsTracked = false, CreatedAt = DateTime.Now };
                    try
                    {
                        await CreateTag(tag);
                    }
                    catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
                    {
                        // Silently ignore duplicate key errors - tag was created by another thread
                        tag = await _tagsCollection.Find(e => e.Name == name).FirstOrDefaultAsync();
                    }
                    catch (MongoWriteException)
                    {
                        // Silently ignore any other MongoDB write errors and retry fetch
                        tag = await _tagsCollection.Find(e => e.Name == name).FirstOrDefaultAsync();
                    }
                }

                return tag;
            }
            catch (MongoException)
            {
                // If all else fails, try one more time to get the tag
                return await _tagsCollection.Find(e => e.Name == name).FirstOrDefaultAsync()
                    ?? new Tag { Name = name, IsTracked = false, CreatedAt = DateTime.Now };
            }
        }

        public async Task UpdateTag(Tag tag)
        {
            await _tagsCollection.ReplaceOneAsync(e => e.Id == tag.Id, tag);
        }
    }
}