using IBudget.Core.DatabaseModel;
using IBudget.Core.Exceptions;
using IBudget.Core.Model;
using IBudget.Core.RepositoryInterfaces;
using LiteDB;
using Tag = IBudget.Core.Model.Tag;

namespace IBudget.Infrastructure.Repositories.LiteDb
{
    public class LiteDbExpenseTagsRepository : IExpenseTagsRepository
    {
        private readonly ILiteCollection<ExpenseTag> _expenseTagsCollection;
        private readonly ILiteCollection<Expense> _expensesCollection;
        private readonly ILiteCollection<Income> _incomeCollection;
        private readonly ILiteCollection<Tag> _tagsCollection;

        public LiteDbExpenseTagsRepository(LiteDbContext context)
        {
            _expenseTagsCollection = context.GetExpenseTagsCollection();
            _expensesCollection = context.GetExpensesCollection();
            _incomeCollection = context.GetIncomeCollection();
            _tagsCollection = context.GetTagsCollection();
            _expenseTagsCollection.EnsureIndex(e => e.Title);
            _expenseTagsCollection.EnsureIndex(e => e.CreatedAt);
        }

        public async Task ClearCollection()
        {
            await Task.Run(() => _expenseTagsCollection.DeleteAll());
        }

        public async Task<ExpenseTag> CreateExpenseTag(ExpenseTag expenseTag)
        {
            await Task.Run(() => _expenseTagsCollection.Insert(expenseTag));
            return expenseTag;
        }

        public async Task DeleteExpenseTagById(MongoDB.Bson.ObjectId id)
        {
            await Task.Run(() => _expenseTagsCollection.Delete(new LiteDB.BsonValue(id.ToString())));
        }

        public async Task DeleteExpenseTagByTitle(string title)
        {
            await Task.Run(() => _expenseTagsCollection.DeleteMany(e => e.Title == title));
        }

        public async Task<List<ExpenseTag>> GetAllExpenseTags()
        {
            return await Task.Run(() => 
                _expenseTagsCollection.Query()
                    .OrderByDescending(e => e.CreatedAt)
                    .ToList());
        }

        public async Task<ExpenseTag> GetExpenseTagById(MongoDB.Bson.ObjectId id)
        {
            return await Task.Run(() => _expenseTagsCollection.FindById(new LiteDB.BsonValue(id.ToString())));
        }

        public async Task<PaginatedResponse<ExpenseTag>> GetExpenseTagByPage(int pageNumber)
        {
            var pageSize = 10;
            var skip = (pageNumber - 1) * pageSize;
            
            return await Task.Run(() =>
            {
                var totalDataCount = _expenseTagsCollection.Count();
                var totalPageCount = (int)Math.Ceiling((double)totalDataCount / pageSize);
                var data = _expenseTagsCollection.Query()
                    .OrderByDescending(e => e.CreatedAt)
                    .Offset(skip)
                    .Limit(pageSize)
                    .ToList();

                return new PaginatedResponse<ExpenseTag>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalDataCount = totalDataCount,
                    TotalPageCount = totalPageCount,
                    Data = data
                };
            });
        }

        public async Task<ExpenseTag> GetExpenseTagByTitle(string title)
        {
            return await Task.Run(() => 
                _expenseTagsCollection.FindOne(e => e.Title.Equals(title, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<ExpenseTag> UpdateExpenseTag(ExpenseTag expenseTag)
        {
            await Task.Run(() =>
            {
                var existingTag = _expenseTagsCollection.FindOne(e => e.Title == expenseTag.Title);
                if (existingTag != null)
                {
                    existingTag.Tags = expenseTag.Tags;
                    _expenseTagsCollection.Update(existingTag);
                }

                var tag = _tagsCollection.FindOne(t => t.Name == expenseTag.Tags[0])
                    ?? throw new RecordNotFoundException();

                // Update all expenses with matching Notes
                var expensesToUpdate = _expensesCollection.Find(e => e.Notes == expenseTag.Title);
                foreach (var expense in expensesToUpdate)
                {
                    expense.Tags = [tag];
                    _expensesCollection.Update(expense);
                }

                // Update all income with matching Source
                var incomesToUpdate = _incomeCollection.Find(i => i.Source == expenseTag.Title);
                foreach (var income in incomesToUpdate)
                {
                    income.Tags = [tag];
                    _incomeCollection.Update(income);
                }
            });

            return expenseTag;
        }
    }
}
