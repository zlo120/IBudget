using IBudget.Core.DatabaseModel;
using IBudget.Core.Exceptions;
using IBudget.Core.Model;
using IBudget.Core.RepositoryInterfaces;
using LiteDB;
using LiteDB.Async;
using Tag = IBudget.Core.Model.Tag;

namespace IBudget.Infrastructure.Repositories.LiteDb
{
    public class LiteDbExpenseTagsRepository : IExpenseTagsRepository
    {
        private readonly ILiteCollectionAsync<ExpenseTag> _expenseTagsCollection;
        private readonly ILiteCollectionAsync<Expense> _expensesCollection;
        private readonly ILiteCollectionAsync<Income> _incomeCollection;
        private readonly ILiteCollectionAsync<Tag> _tagsCollection;

        public LiteDbExpenseTagsRepository(LiteDbContext context)
        {
            _expenseTagsCollection = context.GetExpenseTagsCollection();
            _expensesCollection = context.GetExpensesCollection();
            _incomeCollection = context.GetIncomeCollection();
            _tagsCollection = context.GetTagsCollection();
            _expenseTagsCollection.EnsureIndexAsync(e => e.Title);
            _expenseTagsCollection.EnsureIndexAsync(e => e.CreatedAt);
        }

        public async Task ClearCollection()
        {
            await _expenseTagsCollection.DeleteAllAsync();
        }

        public async Task<ExpenseTag> CreateExpenseTag(ExpenseTag expenseTag)
        {
            try
            {
                await _expenseTagsCollection.InsertAsync(expenseTag);
            }
            catch (LiteException ex) when (ex.ErrorCode == LiteException.INDEX_DUPLICATE_KEY)
            {
                // Silently ignore duplicate key errors
            }
            return expenseTag;
        }

        public async Task DeleteExpenseTagById(MongoDB.Bson.ObjectId id)
        {
            await _expenseTagsCollection.DeleteAsync(new LiteDB.BsonValue(id.ToString()));
        }

        public async Task DeleteExpenseTagByTitle(string title)
        {
            await _expenseTagsCollection.DeleteManyAsync(e => e.Title == title);
        }

        public async Task<List<ExpenseTag>> GetAllExpenseTags()
        {
            return await _expenseTagsCollection.Query().OrderByDescending(e => e.CreatedAt).ToListAsync();
        }

        public async Task<ExpenseTag> GetExpenseTagById(MongoDB.Bson.ObjectId id)
        {
            return await _expenseTagsCollection.FindByIdAsync(new LiteDB.BsonValue(id.ToString()));
        }

        public async Task<PaginatedResponse<ExpenseTag>> GetExpenseTagByPage(int pageNumber)
        {
            var pageSize = 10;
            var skip = (pageNumber - 1) * pageSize;

            var totalDataCount = await _expenseTagsCollection.CountAsync();
            var totalPageCount = (int)Math.Ceiling((double)totalDataCount / pageSize);
            var data = await _expenseTagsCollection.Query()
                .OrderByDescending(e => e.CreatedAt)
                .Offset(skip)
                .Limit(pageSize)
                .ToListAsync();

            var hasMoreData = pageNumber < totalPageCount;
            return new PaginatedResponse<ExpenseTag>
            {
                HasMoreData = hasMoreData,
                Data = data
            };
        }

        public async Task<ExpenseTag> GetExpenseTagByTitle(string title)
        {
            return await _expenseTagsCollection.FindOneAsync(e => e.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<List<ExpenseTag>> Search(string searchString)
        {
            // Search for ExpenseTags where either the title contains the search string OR any tag contains it
            return [..await _expenseTagsCollection.FindAsync(e => 
                e.Title.Contains(searchString, StringComparison.CurrentCultureIgnoreCase) || 
                e.Tags.Any(tag => tag.Contains(searchString, StringComparison.CurrentCultureIgnoreCase)))];
        }

        public async Task<ExpenseTag> UpdateExpenseTag(ExpenseTag expenseTag)
        {
            var existingTag = await _expenseTagsCollection.FindOneAsync(e => e.Title == expenseTag.Title);
            if (existingTag is not null)
            {
                existingTag.Tags = expenseTag.Tags;
                await _expenseTagsCollection.UpdateAsync(existingTag);
            }

            var tag = await _tagsCollection.FindOneAsync(t => t.Name == expenseTag.Tags[0])
                ?? throw new RecordNotFoundException();

            // Update all expenses with matching Notes
            var expensesToUpdate = await _expensesCollection.FindAsync(e => e.Notes == expenseTag.Title);
            foreach (var expense in expensesToUpdate)
            {
                expense.Tags = [tag];
                await _expensesCollection.UpdateAsync(expense);
            }

            // Update all income with matching Source
            var incomesToUpdate = await _incomeCollection.FindAsync(i => i.Source == expenseTag.Title);
            foreach (var income in incomesToUpdate)
            {
                income.Tags = [tag];
                await _incomeCollection.UpdateAsync(income);
            }

            return expenseTag;
        }
    }
}
