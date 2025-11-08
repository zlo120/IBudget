using IBudget.Core.Enums;
using IBudget.Core.Model;
using IBudget.Core.RepositoryInterfaces;
using IBudget.Infrastructure.Utils;
using LiteDB;

namespace IBudget.Infrastructure.Repositories.LiteDb
{
    public class LiteDbImportExportRepository(LiteDbContext context) : IImportExportRepository
    {
        private readonly ILiteCollection<ExpenseRuleTag> _expenseRuleTagsCollection = context.GetExpenseRuleTagsCollection();
        private readonly ILiteCollection<ExpenseTag> _expenseTagsCollection = context.GetExpenseTagsCollection();
        private readonly ILiteCollection<Expense> _expensesCollection = context.GetExpensesCollection();
        private readonly ILiteCollection<Income> _incomeCollection = context.GetIncomeCollection();
        private readonly ILiteCollection<Tag> _tagsCollection = context.GetTagsCollection();
        private readonly ILiteCollection<FinancialGoal> _financialGoalsCollection = context.GetFinancialGoalsCollection();
        public Task<string> ExportData()
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            var exportDirectory = ExportUtils.GetExportCollectionPath("LiteDb", timestamp);

            ExportUtils.ExportLiteDbCollectionToFile(DatabaseCollections.ExpenseRuleTags, timestamp, _expenseRuleTagsCollection);
            ExportUtils.ExportLiteDbCollectionToFile(DatabaseCollections.ExpenseTags, timestamp, _expenseTagsCollection);
            ExportUtils.ExportLiteDbCollectionToFile(DatabaseCollections.Expenses, timestamp, _expensesCollection);
            ExportUtils.ExportLiteDbCollectionToFile(DatabaseCollections.Income, timestamp, _incomeCollection);
            ExportUtils.ExportLiteDbCollectionToFile(DatabaseCollections.Tags, timestamp, _tagsCollection);
            ExportUtils.ExportLiteDbCollectionToFile(DatabaseCollections.FinancialGoals, timestamp, _financialGoalsCollection);

            return Task.FromResult(exportDirectory);
        }
    }
}
