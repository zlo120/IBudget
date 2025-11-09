using System.Text.Json;
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

        public async Task ImportData(string filePath)
        {
            // Read the JSON file
            var jsonContent = await File.ReadAllTextAsync(filePath);

            // Configure JSON serializer options with custom converters
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters =
                {
                    new ObjectIdJsonConverter(),
                    new NullableObjectIdJsonConverter()
                }
            };

            // First, deserialize to get the collection name
            using var doc = JsonDocument.Parse(jsonContent);
            var root = doc.RootElement;
            var collectionName = root.GetProperty("CollectionName").GetString();

            // Import data based on collection name
            switch (collectionName)
            {
                case var name when name == DatabaseCollections.ExpenseRuleTags:
                    var expenseRuleTagsData = JsonSerializer.Deserialize<ExportFile<ExpenseRuleTag>>(jsonContent, options);
                    await ImportUtils.ImportCollectionIntoMongoDb(_expenseRuleTagsCollection, expenseRuleTagsData!.Data);
                    break;

                case var name when name == DatabaseCollections.ExpenseTags:
                    var expenseTagsData = JsonSerializer.Deserialize<ExportFile<ExpenseTag>>(jsonContent, options);
                    await ImportUtils.ImportCollectionIntoMongoDb(_expenseTagsCollection, expenseTagsData!.Data);
                    break;

                case var name when name == DatabaseCollections.Expenses:
                    var expensesData = JsonSerializer.Deserialize<ExportFile<Expense>>(jsonContent, options);
                    await ImportUtils.ImportCollectionIntoMongoDb(_expensesCollection, expensesData!.Data);
                    break;

                case var name when name == DatabaseCollections.Income:
                    var incomeData = JsonSerializer.Deserialize<ExportFile<Income>>(jsonContent, options);
                    await ImportUtils.ImportCollectionIntoMongoDb(_incomeCollection, incomeData!.Data);
                    break;

                case var name when name == DatabaseCollections.Tags:
                    var tagsData = JsonSerializer.Deserialize<ExportFile<Tag>>(jsonContent, options);
                    await ImportUtils.ImportCollectionIntoMongoDb(_tagsCollection, tagsData!.Data);
                    break;

                case var name when name == DatabaseCollections.FinancialGoals:
                    var financialGoalsData = JsonSerializer.Deserialize<ExportFile<FinancialGoal>>(jsonContent, options);
                    await ImportUtils.ImportCollectionIntoMongoDb(_financialGoalsCollection, financialGoalsData!.Data);
                    break;

                default:
                    throw new ArgumentException($"Unknown collection name: {collectionName}");
            }
        }
    }
}
