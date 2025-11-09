using IBudget.Core.RepositoryInterfaces;
using LiteDB;
using LiteDB.Async;
using Tag = IBudget.Core.Model.Tag;

namespace IBudget.Infrastructure.Repositories.LiteDb
{
    public class LiteDbTagsRepository : ITagsRepository
    {
        private readonly ILiteCollectionAsync<Tag> _tagsCollection;
        private readonly IExpenseTagsRepository _expenseTagsRepository;
        private readonly IExpenseRuleTagsRepository _expenseRuleTagsRepository;

        public LiteDbTagsRepository(
            LiteDbContext context, 
            IExpenseTagsRepository expenseTagsRepository, 
            IExpenseRuleTagsRepository expenseRuleTagsRepository)
        {
            _tagsCollection = context.GetTagsCollection();
            _expenseTagsRepository = expenseTagsRepository;
            _expenseRuleTagsRepository = expenseRuleTagsRepository;
        }

        public async Task ClearCollection()
        {
            await _tagsCollection.DeleteManyAsync(t => t.Name != "ignored");
        }

        public async Task CreateTag(Tag tag)
        {
            try
            {
                await _tagsCollection.InsertAsync(tag);
            }
            catch (LiteException ex) when (ex.ErrorCode == LiteException.INDEX_DUPLICATE_KEY || ex.Message.Contains("duplicate") || ex.Message.Contains("unique"))
            {
                // Silently ignore duplicate key errors
            }
            catch (LiteException)
            {
                // Silently ignore any other LiteDB errors during insert (per user request)
            }
        }

        public async Task DeleteTagById(MongoDB.Bson.ObjectId id)
        {
            await _tagsCollection.DeleteAsync(new BsonValue(id.ToString()));
        }

        public async Task DeleteTagByName(string name)
        {
            await _tagsCollection.DeleteManyAsync(t => t.Name == name);
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

            tagNames = [.. (await _expenseTagsRepository.GetAllExpenseTags())
                .Where(e => e.Title.Contains(description, StringComparison.InvariantCultureIgnoreCase))
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
            return [.. await _tagsCollection.FindAllAsync()];
        }

        public async Task<Tag> GetOrCreateTagByName(string name)
        {
            try
            {
                var tag = await _tagsCollection.FindOneAsync(t => t.Name == name);
                if (tag is null)
                {
                    tag = new Tag { Name = name, IsTracked = false, CreatedAt = DateTime.Now };
                    try
                    {
                        await CreateTag(tag);
                    }
                    catch (LiteException ex) when (ex.ErrorCode == LiteException.INDEX_DUPLICATE_KEY || ex.Message.Contains("duplicate") || ex.Message.Contains("unique"))
                    {
                        // Silently ignore duplicate key errors - tag was created by another thread
                        tag = await _tagsCollection.FindOneAsync(t => t.Name == name);
                    }
                    catch (LiteException)
                    {
                        // Silently ignore any other LiteDB errors and retry fetch
                        tag = await _tagsCollection.FindOneAsync(t => t.Name == name);
                    }
                }
                return tag;
            }
            catch (LiteException)
            {
                // If all else fails, try one more time to get the tag
                return await _tagsCollection.FindOneAsync(t => t.Name == name) 
                    ?? new Tag { Name = name, IsTracked = false, CreatedAt = DateTime.Now };
            }
        }

        public async Task UpdateTag(Tag tag)
        {
            await _tagsCollection.UpdateAsync(tag);
        }
    }
}
