using IBudget.Core.DatabaseModel;
using IBudget.Core.Exceptions;
using IBudget.Core.Model;
using IBudget.Core.RepositoryInterfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using Tag = IBudget.Core.Model.Tag;

namespace IBudget.Infrastructure.Repositories
{
    public class ExpenseTagsRepository(MongoDbContext context) : IExpenseTagsRepository
    {
        private readonly IMongoCollection<ExpenseTag> _expenseTagsCollection = context.GetExpenseTagsCollection();
        private readonly IMongoCollection<Expense> _expensesCollection = context.GetExpensesCollection();
        private readonly IMongoCollection<Income> _incomeCollection = context.GetIncomeCollection();
        private readonly IMongoCollection<Tag> _tagsCollection = context.GetTagsCollection();

        public async Task ClearCollection()
        {
            await _expenseTagsCollection.DeleteManyAsync(FilterDefinition<ExpenseTag>.Empty);
        }

        public async Task<ExpenseTag> CreateExpenseTag(ExpenseTag expenseTag)
        {
            try
            {
                await _expenseTagsCollection.InsertOneAsync(expenseTag);
            }
            catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
            {
                // Silently ignore duplicate key errors
            }
            return expenseTag;
        }

        public async Task DeleteExpenseTagById(ObjectId id)
        {
            await _expenseTagsCollection.DeleteOneAsync(e => e.Id == id);
        }

        public async Task DeleteExpenseTagByTitle(string title)
        {
            await _expenseTagsCollection.DeleteOneAsync(e => e.Title == title);
        }

        public async Task<List<ExpenseTag>> GetAllExpenseTags()
        {
            return await _expenseTagsCollection.Find(FilterDefinition<ExpenseTag>.Empty)
                .SortByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<ExpenseTag> GetExpenseTagById(ObjectId id)
        {
            return await _expenseTagsCollection.Find(e => e.Id == id).FirstOrDefaultAsync();
        }

        public async Task<PaginatedResponse<ExpenseTag>> GetExpenseTagByPage(int pageNumber)
        {
            var pageSize = 10;
            var skip = (pageNumber - 1) * pageSize;
            var totalDataCount = _expenseTagsCollection.CountDocuments(FilterDefinition<ExpenseTag>.Empty);
            var totalPageCount = (int)Math.Ceiling((double)totalDataCount / pageSize);
            var data = await _expenseTagsCollection.Find(FilterDefinition<ExpenseTag>.Empty)
                .SortByDescending(e => e.CreatedAt)
                .Skip(skip)
                .Limit(pageSize)
                .ToListAsync();

            return new PaginatedResponse<ExpenseTag>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalDataCount = (int)totalDataCount,
                TotalPageCount = totalPageCount,
                Data = data
            };
        }

        public async Task<ExpenseTag> GetExpenseTagByTitle(string title)
        {
            return await _expenseTagsCollection.Find(e => e.Title.Equals(title, StringComparison.OrdinalIgnoreCase)).FirstOrDefaultAsync();
        }

        public async Task<ExpenseTag> UpdateExpenseTag(ExpenseTag expenseTag)
        {
            var expenseTagFilter = Builders<ExpenseTag>.Filter.Eq(e => e.Title, expenseTag.Title);
            await _expenseTagsCollection.UpdateOneAsync(
                expenseTagFilter,
                Builders<ExpenseTag>.Update
                    .Set(e => e.Tags, expenseTag.Tags)
            );
            var tag = await _tagsCollection.Find(e => e.Name == expenseTag.Tags[0]).FirstOrDefaultAsync()
                ?? throw new RecordNotFoundException();

            var expenseFilter = Builders<Expense>.Filter.Eq("Notes", expenseTag.Title);
            await _expensesCollection.UpdateManyAsync(
                expenseFilter,
                Builders<Expense>.Update
                    .Set(e => e.Tags, [tag])
            );

            var incomeFilter = Builders<Income>.Filter.Eq("Source", expenseTag.Title);
            await _incomeCollection.UpdateManyAsync(
                incomeFilter,
                Builders<Income>.Update
                    .Set(i => i.Tags, [tag])
            );
            return expenseTag;
        }

        // Replace all ExpenseTags that match a condition
        public async Task<int> ReplaceExpenseTagsWhere(FilterDefinition<ExpenseTag> filter, ExpenseTag replacement)
        {
            var result = await _expenseTagsCollection.UpdateManyAsync(
                filter,
                Builders<ExpenseTag>.Update
                    .Set(e => e.Title, replacement.Title)
                    .Set(e => e.Tags, replacement.Tags)
            );
            return (int)result.ModifiedCount;
        }

        public async Task<List<ExpenseTag>> Search(string searchString)
        {
            return await _expenseTagsCollection.Find(e => e.Title.Contains(searchString, StringComparison.CurrentCultureIgnoreCase)).ToListAsync();
        }
    }
}
