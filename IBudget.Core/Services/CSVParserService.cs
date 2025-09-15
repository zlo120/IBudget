using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using IBudget.Core.Utils;

namespace IBudget.Core.Services
{
    public class CSVParserService(IExpenseTagService expenseTagService, IExpenseRuleTagService expenseRuleTagService) : ICSVParserService
    {
        private readonly IExpenseTagService _expenseTagService = expenseTagService;
        private readonly IExpenseRuleTagService _expenseRuleTagService = expenseRuleTagService;
        public List<FormattedFinancialCSV> ParseCSV(string csvFilePath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            };

            using var reader = new StreamReader(csvFilePath);
            using var csv = new CsvReader(reader, config);
            var records = csv.GetRecords<FinancialCSV>();
            var formattedRecords = new List<FormattedFinancialCSV>();
            foreach (var record in records)
            {
                formattedRecords.Add(new FormattedFinancialCSV()
                {
                    Date = CsvFormatter.FormatDate(record.Date),
                    Description = CsvFormatter.FormatDescription(record.Description),
                    Amount = record.Amount
                });
            }
            return formattedRecords;
        }
        public async Task<List<FormattedFinancialCSV>> FindUntagged(List<FormattedFinancialCSV> records)
        {
            records = records.Distinct().ToList();
            List<ExpenseTag> expenseTags = await _expenseTagService.GetAllExpenseTags();
            List<ExpenseRuleTag> expenseRuleTags = await _expenseRuleTagService.GetAllExpenseRuleTags();

            var untaggedRecords = new List<FormattedFinancialCSV>();
            foreach (var record in records)
            {
                var expenseTagsMatch = expenseTags
                    .Where(expenseDictionary => expenseDictionary.Title.Equals(record.Description, StringComparison.InvariantCultureIgnoreCase))
                    .FirstOrDefault();

                var expenseRuleTagsMatch = expenseRuleTags
                    .Where(ruleDictionary =>
                        record.Description.Contains(ruleDictionary.Rule, StringComparison.InvariantCultureIgnoreCase)
                        )
                    .FirstOrDefault();

                if (expenseRuleTagsMatch is null && expenseRuleTagsMatch is null)
                    untaggedRecords.Add(record);
            }

            return [.. untaggedRecords.Distinct()];
        }
    }
}