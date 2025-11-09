using IBudget.Core.Enums;
using IBudget.Core.Model;
using IBudget.Core.RepositoryInterfaces;
using IBudget.Infrastructure.Utils;
using LiteDB.Async;
using Tag = IBudget.Core.Model.Tag;

namespace IBudget.Infrastructure.Repositories.LiteDb
{
    public class LiteDbImportExportRepository(LiteDbContext context) : IImportExportRepository
    {
        private readonly ILiteCollectionAsync<ExpenseRuleTag> _expenseRuleTagsCollection = context.GetExpenseRuleTagsCollection();
        private readonly ILiteCollectionAsync<ExpenseTag> _expenseTagsCollection = context.GetExpenseTagsCollection();
        private readonly ILiteCollectionAsync<Expense> _expensesCollection = context.GetExpensesCollection();
        private readonly ILiteCollectionAsync<Income> _incomeCollection = context.GetIncomeCollection();
        private readonly ILiteCollectionAsync<Tag> _tagsCollection = context.GetTagsCollection();
        private readonly ILiteCollectionAsync<FinancialGoal> _financialGoalsCollection = context.GetFinancialGoalsCollection();

        public async Task<string> ExportData()
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            var exportDirectory = ExportUtils.GetExportCollectionPath("LiteDb", timestamp);

            await Task.WhenAll(
                ExportUtils.ExportLiteDbCollectionToFileAsync(DatabaseCollections.ExpenseRuleTags, timestamp, _expenseRuleTagsCollection),
                ExportUtils.ExportLiteDbCollectionToFileAsync(DatabaseCollections.ExpenseTags, timestamp, _expenseTagsCollection),
                ExportUtils.ExportLiteDbCollectionToFileAsync(DatabaseCollections.Expenses, timestamp, _expensesCollection),
                ExportUtils.ExportLiteDbCollectionToFileAsync(DatabaseCollections.Income, timestamp, _incomeCollection),
                ExportUtils.ExportLiteDbCollectionToFileAsync(DatabaseCollections.Tags, timestamp, _tagsCollection),
                ExportUtils.ExportLiteDbCollectionToFileAsync(DatabaseCollections.FinancialGoals, timestamp, _financialGoalsCollection)
            );

            return exportDirectory;
        }

        public Task ImportData(string filePath)
        {
            // Read the JSON file
            var jsonContent = File.ReadAllText(filePath);

            // Configure JSON serializer options with custom converters
            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters =
                {
                    new ObjectIdJsonConverter(),
                    new NullableObjectIdJsonConverter()
                }
            };

            // First, deserialize to get the collection name
            using var doc = System.Text.Json.JsonDocument.Parse(jsonContent);
            var root = doc.RootElement;
            var collectionName = root.GetProperty("CollectionName").GetString();

            // Import data based on collection name
            switch (collectionName)
            {
                case var name when name == DatabaseCollections.ExpenseRuleTags:
                    var expenseRuleTagsData = System.Text.Json.JsonSerializer.Deserialize<ExportFile<ExpenseRuleTag>>(jsonContent, options);
                    ImportUtils.ImportCollectionIntoLiteDb(_expenseRuleTagsCollection, expenseRuleTagsData!.Data);
                    break;

                case var name when name == DatabaseCollections.ExpenseTags:
                    var expenseTagsData = System.Text.Json.JsonSerializer.Deserialize<ExportFile<ExpenseTag>>(jsonContent, options);
                    ImportUtils.ImportCollectionIntoLiteDb(_expenseTagsCollection, expenseTagsData!.Data);
                    break;

                case var name when name == DatabaseCollections.Expenses:
                    var expensesData = System.Text.Json.JsonSerializer.Deserialize<ExportFile<Expense>>(jsonContent, options);
                    ImportUtils.ImportCollectionIntoLiteDb(_expensesCollection, expensesData!.Data);
                    break;

                case var name when name == DatabaseCollections.Income:
                    var incomeData = System.Text.Json.JsonSerializer.Deserialize<ExportFile<Income>>(jsonContent, options);
                    ImportUtils.ImportCollectionIntoLiteDb(_incomeCollection, incomeData!.Data);
                    break;

                case var name when name == DatabaseCollections.Tags:
                    var tagsData = System.Text.Json.JsonSerializer.Deserialize<ExportFile<Tag>>(jsonContent, options);
                    ImportUtils.ImportCollectionIntoLiteDb(_tagsCollection, tagsData!.Data);
                    break;

                case var name when name == DatabaseCollections.FinancialGoals:
                    var financialGoalsData = System.Text.Json.JsonSerializer.Deserialize<ExportFile<FinancialGoal>>(jsonContent, options);
                    ImportUtils.ImportCollectionIntoLiteDb(_financialGoalsCollection, financialGoalsData!.Data);
                    break;

                default:
                    throw new ArgumentException($"Unknown collection name: {collectionName}");
            }

            return Task.CompletedTask;
        }
    }
}
