using IBudget.Core.Enums;
using IBudget.Core.Model;
using IBudget.Core.RepositoryInterfaces;
using IBudget.Infrastructure.Utils;
using MongoDB.Driver;
using Tag = IBudget.Core.Model.Tag;

namespace IBudget.Infrastructure.Repositories
{
    public class ImportExportRepository(MongoDbContext context) : IImportExportRepository
    {
        private readonly IMongoCollection<ExpenseRuleTag> _expenseRuleTagsCollection = context.GetExpenseRuleTagsCollection();
        private readonly IMongoCollection<ExpenseTag> _expenseTagsCollection = context.GetExpenseTagsCollection();
        private readonly IMongoCollection<Expense> _expensesCollection = context.GetExpensesCollection();
        private readonly IMongoCollection<Income> _incomeCollection = context.GetIncomeCollection();
        private readonly IMongoCollection<Tag> _tagsCollection = context.GetTagsCollection();
        private readonly IMongoCollection<FinancialGoal> _financialGoalsCollection = context.GetFinancialGoalsCollection();
        public async Task<string> ExportData()
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            var exportDirectory = ExportUtils.GetExportCollectionPath("MongoDb", timestamp);

            await Task.WhenAll(
                ExportUtils.ExportMongoCollectionToFile(DatabaseCollections.ExpenseRuleTags, timestamp, _expenseRuleTagsCollection),
                ExportUtils.ExportMongoCollectionToFile(DatabaseCollections.ExpenseTags, timestamp, _expenseTagsCollection),
                ExportUtils.ExportMongoCollectionToFile(DatabaseCollections.Expenses, timestamp, _expensesCollection),
                ExportUtils.ExportMongoCollectionToFile(DatabaseCollections.Income, timestamp, _incomeCollection),
                ExportUtils.ExportMongoCollectionToFile(DatabaseCollections.Tags, timestamp, _tagsCollection),
                ExportUtils.ExportMongoCollectionToFile(DatabaseCollections.FinancialGoals, timestamp, _financialGoalsCollection)
            );

            return exportDirectory;
        }
    }
}
