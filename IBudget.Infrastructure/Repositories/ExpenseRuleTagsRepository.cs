using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using IBudget.Core.DatabaseModel;
using IBudget.Core.Exceptions;
using IBudget.Core.Model;
using IBudget.Core.RepositoryInterfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using Tag = IBudget.Core.Model.Tag;

namespace IBudget.Infrastructure.Repositories
{
    public class ExpenseRuleTagsRepository(MongoDbContext context) : IExpenseRuleTagsRepository
    {
        private readonly IMongoCollection<ExpenseRuleTag> _expenseRuleTagsCollection = context.GetExpenseRuleTagsCollection();
        private readonly IMongoCollection<Expense> _expensesCollection = context.GetExpensesCollection();
        private readonly IMongoCollection<Income> _incomesCollection = context.GetIncomeCollection();
        private readonly IMongoCollection<Tag> _tagsCollection = context.GetTagsCollection();
        public async Task<ExpenseRuleTag> CreateExpenseRuleTag(ExpenseRuleTag expenseRuleTag)
        {
            await _expenseRuleTagsCollection.InsertOneAsync(expenseRuleTag);
            return expenseRuleTag;
        }

        public async Task DeleteExpenseRuleTagById(ObjectId id)
        {
            await _expenseRuleTagsCollection.DeleteOneAsync(e => e.Id == id);
        }

        public async Task DeleteExpenseRuleTagByRule(string rule)
        {
            await _expenseRuleTagsCollection.DeleteOneAsync(e => e.Rule == rule);
        }

        public async Task<List<ExpenseRuleTag>> GetAllExpenseRuleTags()
        {
            return await _expenseRuleTagsCollection.Find(FilterDefinition<ExpenseRuleTag>.Empty)
                .SortByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<ExpenseRuleTag> GetExpenseRuleTagById(ObjectId id)
        {
            return await _expenseRuleTagsCollection.Find(e => e.Id == id).FirstOrDefaultAsync();
        }

        public async Task<PaginatedResponse<ExpenseRuleTag>> GetExpenseRuleTagByPage(int pageNumber)
        {
            var pageSize = 10;
            var skip = (pageNumber - 1) * pageSize;
            var totalDataCount = _expenseRuleTagsCollection.CountDocuments(FilterDefinition<ExpenseRuleTag>.Empty);
            var totalPageCount = (int)Math.Ceiling((double)totalDataCount / pageSize);
            var data = await _expenseRuleTagsCollection.Find(FilterDefinition<ExpenseRuleTag>.Empty)
                .SortByDescending(e => e.CreatedAt)
                .Skip(skip)
                .Limit(pageSize)
                .ToListAsync();

            return new PaginatedResponse<ExpenseRuleTag>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalDataCount = (int)totalDataCount,
                TotalPageCount = totalPageCount,
                Data = data
            };
        }

        public async Task<ExpenseRuleTag?> GetExpenseRuleTagByDescription(string description)
        {
            return await _expenseRuleTagsCollection.Find(e => description.ToLower().Contains(e.Rule.ToLower())).FirstOrDefaultAsync();
        }

        public async Task<ExpenseRuleTag> UpdateExpenseRuleTag(ExpenseRuleTag expenseRuleTag)
        {
            var expenseRuleTagFilter = Builders<ExpenseRuleTag>.Filter.Eq(e => e.Rule, expenseRuleTag.Rule);
            await _expenseRuleTagsCollection.UpdateOneAsync(
                expenseRuleTagFilter,
                Builders<ExpenseRuleTag>.Update
                    .Set(e => e.Tags, expenseRuleTag.Tags)
            );
            var tag = await _tagsCollection.Find(e => e.Name == expenseRuleTag.Tags[0]).FirstOrDefaultAsync()
                ?? throw new RecordNotFoundException();

            var expenseFilter = Builders<Expense>.Filter.Regex("Notes", expenseRuleTag.Rule);
            await _expensesCollection.UpdateManyAsync(
                expenseFilter,
                Builders<Expense>.Update
                    .Set(e => e.Tags, [tag])
            );

            var incomeFilter = Builders<Income>.Filter.Regex("Source", expenseRuleTag.Rule);
            await _incomesCollection.UpdateManyAsync(
                incomeFilter,
                Builders<Income>.Update
                    .Set(i => i.Tags, [tag])
            );

            return expenseRuleTag;
        }

        public async Task ClearCollection()
        {
            await _expenseRuleTagsCollection.DeleteManyAsync(FilterDefinition<ExpenseRuleTag>.Empty);
        }
    }
}
