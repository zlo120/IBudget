using DocumentFormat.OpenXml.Office2010.Excel;
using IBudget.Core.Model;
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

        public async Task<List<string>> FindTagsByDescription(string description)
        {
            var tags = (await _expenseRuleTagsRepository.GetExpenseRuleTagByRule(description)).Tags;
            if (tags.Count > 0) return tags;
            var allExpenseTags = await _expenseTagsRepository.GetAllExpenseTags();
            return [.. allExpenseTags.Where(e => e.Title.Contains(description, StringComparison.InvariantCultureIgnoreCase))
                .SelectMany(e => e.Tags)
                .Distinct()];
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