using IBudget.Core.DatabaseModel;
using IBudget.Core.Exceptions;
using IBudget.Core.Model;
using IBudget.Core.RepositoryInterfaces;
using LiteDB;
using Tag = IBudget.Core.Model.Tag;

namespace IBudget.Infrastructure.Repositories.LiteDb
{
    public class LiteDbExpenseRuleTagsRepository : IExpenseRuleTagsRepository
    {
        private readonly ILiteCollection<ExpenseRuleTag> _expenseRuleTagsCollection;
        private readonly ILiteCollection<Expense> _expensesCollection;
        private readonly ILiteCollection<Income> _incomesCollection;
        private readonly ILiteCollection<Tag> _tagsCollection;

        public LiteDbExpenseRuleTagsRepository(LiteDbContext context)
        {
            _expenseRuleTagsCollection = context.GetExpenseRuleTagsCollection();
            _expensesCollection = context.GetExpensesCollection();
            _incomesCollection = context.GetIncomeCollection();
            _tagsCollection = context.GetTagsCollection();
            _expenseRuleTagsCollection.EnsureIndex(e => e.Rule);
            _expenseRuleTagsCollection.EnsureIndex(e => e.CreatedAt);
        }

        public async Task ClearCollection()
        {
            await Task.Run(() => _expenseRuleTagsCollection.DeleteAll());
        }

        public async Task<ExpenseRuleTag> CreateExpenseRuleTag(ExpenseRuleTag expenseRuleTag)
        {
            await Task.Run(() => _expenseRuleTagsCollection.Insert(expenseRuleTag));
            return expenseRuleTag;
        }

        public async Task DeleteExpenseRuleTagById(MongoDB.Bson.ObjectId id)
        {
            await Task.Run(() => _expenseRuleTagsCollection.Delete(new LiteDB.BsonValue(id.ToString())));
        }

        public async Task DeleteExpenseRuleTagByRule(string rule)
        {
            await Task.Run(() => _expenseRuleTagsCollection.DeleteMany(e => e.Rule == rule));
        }

        public async Task<List<ExpenseRuleTag>> GetAllExpenseRuleTags()
        {
            return await Task.Run(() => 
                _expenseRuleTagsCollection.Query()
                    .OrderByDescending(e => e.CreatedAt)
                    .ToList());
        }

        public async Task<ExpenseRuleTag> GetExpenseRuleTagById(MongoDB.Bson.ObjectId id)
        {
            return await Task.Run(() => _expenseRuleTagsCollection.FindById(new LiteDB.BsonValue(id.ToString())));
        }

        public async Task<PaginatedResponse<ExpenseRuleTag>> GetExpenseRuleTagByPage(int pageNumber)
        {
            var pageSize = 10;
            var skip = (pageNumber - 1) * pageSize;
            
            return await Task.Run(() =>
            {
                var totalDataCount = _expenseRuleTagsCollection.Count();
                var totalPageCount = (int)Math.Ceiling((double)totalDataCount / pageSize);
                var data = _expenseRuleTagsCollection.Query()
                    .OrderByDescending(e => e.CreatedAt)
                    .Offset(skip)
                    .Limit(pageSize)
                    .ToList();

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
            return await Task.Run(() =>
            {
                var allRuleTags = _expenseRuleTagsCollection.FindAll();
                return allRuleTags.FirstOrDefault(e => 
                    description.ToLower().Contains(e.Rule.ToLower()));
            });
        }

        public async Task<ExpenseRuleTag> UpdateExpenseRuleTag(ExpenseRuleTag expenseRuleTag)
        {
            await Task.Run(() =>
            {
                var existingTag = _expenseRuleTagsCollection.FindOne(e => e.Rule == expenseRuleTag.Rule);
                if (existingTag != null)
                {
                    existingTag.Tags = expenseRuleTag.Tags;
                    _expenseRuleTagsCollection.Update(existingTag);
                }

                var tag = _tagsCollection.FindOne(t => t.Name == expenseRuleTag.Tags[0])
                    ?? throw new RecordNotFoundException();

                // Update all expenses with Notes matching the rule
                var expensesToUpdate = _expensesCollection.Find(e => 
                    e.Notes != null && e.Notes.ToLower().Contains(expenseRuleTag.Rule.ToLower()));
                foreach (var expense in expensesToUpdate)
                {
                    expense.Tags = [tag];
                    _expensesCollection.Update(expense);
                }

                // Update all income with Source matching the rule
                var incomesToUpdate = _incomesCollection.Find(i => 
                    i.Source != null && i.Source.ToLower().Contains(expenseRuleTag.Rule.ToLower()));
                foreach (var income in incomesToUpdate)
                {
                    income.Tags = [tag];
                    _incomesCollection.Update(income);
                }
            });

            return expenseRuleTag;
        }
    }
}
