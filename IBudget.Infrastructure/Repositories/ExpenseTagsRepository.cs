using DocumentFormat.OpenXml.Office2010.Excel;
using IBudget.Core.DatabaseModel;
using IBudget.Core.Model;
using IBudget.Core.RepositoryInterfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace IBudget.Infrastructure.Repositories
{
    public class ExpenseTagsRepository(MongoDbContext context) : IExpenseTagsRepository
    {
        private readonly IMongoCollection<ExpenseTag> _expenseTagsCollection = context.GetExpenseTagsCollection();

        public async Task ClearCollection()
        {
            await _expenseTagsCollection.DeleteManyAsync(FilterDefinition<ExpenseTag>.Empty);
        }

        public async Task<ExpenseTag> CreateExpenseTag(ExpenseTag expenseTag)
        {
            await _expenseTagsCollection.InsertOneAsync(expenseTag);
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
            return await _expenseTagsCollection.Find(e => e.Title == title).FirstOrDefaultAsync();
        }

        public async Task<ExpenseTag> UpdateExpenseTag(ExpenseTag expenseTag)
        {
            await _expenseTagsCollection.ReplaceOneAsync(e => e.Id == expenseTag.Id, expenseTag);
            return expenseTag;
        }
    }
}
