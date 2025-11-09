using IBudget.Core.DatabaseModel;
using IBudget.Core.Exceptions;
using IBudget.Core.Model;
using IBudget.Core.RepositoryInterfaces;
using LiteDB;
using LiteDB.Async;
using Tag = IBudget.Core.Model.Tag;

namespace IBudget.Infrastructure.Repositories.LiteDb
{
    public class LiteDbExpenseRuleTagsRepository : IExpenseRuleTagsRepository
    {
        private readonly ILiteCollectionAsync<ExpenseRuleTag> _expenseRuleTagsCollection;
        private readonly ILiteCollectionAsync<Expense> _expensesCollection;
        private readonly ILiteCollectionAsync<Income> _incomesCollection;
        private readonly ILiteCollectionAsync<Tag> _tagsCollection;

        public LiteDbExpenseRuleTagsRepository(LiteDbContext context)
        {
            _expenseRuleTagsCollection = context.GetExpenseRuleTagsCollection();
            _expensesCollection = context.GetExpensesCollection();
            _incomesCollection = context.GetIncomeCollection();
            _tagsCollection = context.GetTagsCollection();
            _expenseRuleTagsCollection.EnsureIndexAsync(e => e.Rule);
            _expenseRuleTagsCollection.EnsureIndexAsync(e => e.CreatedAt);
        }

        public async Task ClearCollection()
        {
            await Task.Run(() => _expenseRuleTagsCollection.DeleteAllAsync());
        }

        public async Task<ExpenseRuleTag> CreateExpenseRuleTag(ExpenseRuleTag expenseRuleTag)
        {
            try
            {
                await Task.Run(() => _expenseRuleTagsCollection.InsertAsync(expenseRuleTag));
            }
            catch (LiteException ex) when (ex.ErrorCode == LiteException.INDEX_DUPLICATE_KEY)
            {
                // Silently ignore duplicate key errors
            }
            return expenseRuleTag;
        }

        public async Task DeleteExpenseRuleTagById(MongoDB.Bson.ObjectId id)
        {
            await Task.Run(() => _expenseRuleTagsCollection.DeleteAsync(new LiteDB.BsonValue(id.ToString())));
        }

        public async Task DeleteExpenseRuleTagByRule(string rule)
        {
            await Task.Run(() => _expenseRuleTagsCollection.DeleteManyAsync(e => e.Rule == rule));
        }

        public async Task<List<ExpenseRuleTag>> GetAllExpenseRuleTags()
        {
            return await _expenseRuleTagsCollection.Query()
                    .OrderByDescending(e => e.CreatedAt)
                    .ToListAsync();
        }

        public async Task<ExpenseRuleTag> GetExpenseRuleTagById(MongoDB.Bson.ObjectId id)
        {
            return await Task.Run(() => _expenseRuleTagsCollection.FindByIdAsync(new LiteDB.BsonValue(id.ToString())));
        }

        public async Task<PaginatedResponse<ExpenseRuleTag>> GetExpenseRuleTagByPage(int pageNumber)
        {
            var pageSize = 10;
            var skip = (pageNumber - 1) * pageSize;

            return await Task.Run(async () =>
            {
                var totalDataCount = await _expenseRuleTagsCollection.CountAsync();
                var totalPageCount = (int)Math.Ceiling((double)totalDataCount / pageSize);
                var data = await _expenseRuleTagsCollection.Query()
                    .OrderByDescending(e => e.CreatedAt)
                    .Offset(skip)
                    .Limit(pageSize)
                    .ToListAsync();

                return new PaginatedResponse<ExpenseRuleTag>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalDataCount = totalDataCount,
                    TotalPageCount = totalPageCount,
                    Data = data
                };
            });
        }

        public async Task<ExpenseRuleTag?> GetExpenseRuleTagByDescription(string description)
        {
            return (await _expenseRuleTagsCollection.FindAllAsync()).FirstOrDefault(e =>
                description.Contains(e.Rule, StringComparison.CurrentCultureIgnoreCase));
        }

        public async Task<ExpenseRuleTag> UpdateExpenseRuleTag(ExpenseRuleTag expenseRuleTag)
        {
            var existingTag = await _expenseRuleTagsCollection.FindOneAsync(e => e.Rule == expenseRuleTag.Rule);
            if (existingTag is not null)
            {
                existingTag.Tags = expenseRuleTag.Tags;
                await _expenseRuleTagsCollection.UpdateAsync(existingTag);
            }

            var tag = await _tagsCollection.FindOneAsync(t => t.Name == expenseRuleTag.Tags[0])
                ?? throw new RecordNotFoundException();

            // Update all expenses with Notes matching the rule
            var expensesToUpdate = await _expensesCollection.FindAsync(e =>
                e.Notes != null && e.Notes.ToLower().Contains(expenseRuleTag.Rule.ToLower()));
            foreach (var expense in expensesToUpdate)
            {
                expense.Tags = [tag];
                await _expensesCollection.UpdateAsync(expense);
            }

            // Update all income with Source matching the rule
            var incomesToUpdate = await _incomesCollection.FindAsync(i =>
                i.Source != null && i.Source.ToLower().Contains(expenseRuleTag.Rule.ToLower()));
            foreach (var income in incomesToUpdate)
            {
                income.Tags = [tag];
                await _incomesCollection.UpdateAsync(income);
            }

            return expenseRuleTag;
        }
    }
}
