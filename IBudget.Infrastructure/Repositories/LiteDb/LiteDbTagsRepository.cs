using IBudget.Core.RepositoryInterfaces;
using LiteDB;
using Tag = IBudget.Core.Model.Tag;

namespace IBudget.Infrastructure.Repositories.LiteDb
{
    public class LiteDbTagsRepository : ITagsRepository
    {
        private readonly ILiteCollection<Tag> _tagsCollection;
        private readonly IExpenseTagsRepository _expenseTagsRepository;
        private readonly IExpenseRuleTagsRepository _expenseRuleTagsRepository;

        public LiteDbTagsRepository(
            LiteDbContext context, 
            IExpenseTagsRepository expenseTagsRepository, 
            IExpenseRuleTagsRepository expenseRuleTagsRepository)
        {
            _tagsCollection = context.GetTagsCollection();
            _tagsCollection.EnsureIndex(t => t.Name);
            _expenseTagsRepository = expenseTagsRepository;
            _expenseRuleTagsRepository = expenseRuleTagsRepository;
        }

        public async Task ClearCollection()
        {
            await Task.Run(() => _tagsCollection.DeleteMany(t => t.Name != "ignored"));
        }

        public async Task CreateTag(Tag tag)
        {
            await Task.Run(() => _tagsCollection.Insert(tag));
        }

        public async Task DeleteTagById(MongoDB.Bson.ObjectId id)
        {
            await Task.Run(() => _tagsCollection.Delete(new LiteDB.BsonValue(id.ToString())));
        }

        public async Task DeleteTagByName(string name)
        {
            await Task.Run(() => _tagsCollection.DeleteMany(t => t.Name == name));
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

            tagNames = [.. (await _expenseTagsRepository.GetAllExpenseTags())
                .Where(e => e.Title.Contains(description, StringComparison.InvariantCultureIgnoreCase))
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
            return await Task.Run(() => _tagsCollection.FindAll().ToList());
        }

        public async Task<Tag> GetTagByName(string name)
        {
            return await Task.Run(() => _tagsCollection.FindOne(t => t.Name == name));
        }

        public async Task UpdateTag(Tag tag)
        {
            await Task.Run(() => _tagsCollection.Update(tag));
        }
    }
}
