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
            var hasHeader = false;
            var userHeaderToPropertyMap = new Dictionary<string, string>
            {
                { "UserDateHeader", "Date" },
                { "UserAmountHeader", "Amount" },
                { "UserDescriptionHeader", "Description" }
            };

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = hasHeader
            };

            using var reader = new StreamReader(csvFilePath);
            using var csv = new CsvReader(reader, config);
            var formattedRecords = new List<FormattedFinancialCSV>();

            if (hasHeader)
            {
                csv.Context.RegisterClassMap(new DynamicFinancialCSVMap(userHeaderToPropertyMap));

                var records = csv.GetRecords<dynamic>();
                foreach (var record in records)
                {
                    var dict = (IDictionary<string, object>)record;
                    // Access fields by header name, e.g. dict["Amount"], dict["Date"], etc.
                    formattedRecords.Add(new FormattedFinancialCSV()
                    {
                        Date = CsvFormatter.FormatDate(record.Date),
                        Description = CsvFormatter.FormatDescription(record.Description),
                        Amount = record.Amount
                    });
                }
            }
            else
            {
                while (csv.Read())
                {
                    var row = csv.Parser.Record;
                    if (row.Length < 3) continue; 

                    // Try parse date
                    if (!DateTime.TryParseExact(row[0], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                    {
                        continue;
                    }

                    if (!double.TryParse(row[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var amount))
                    {
                        continue;
                    }

                    var description = row[2];
                    if (string.IsNullOrWhiteSpace(description))
                    {
                        continue;
                    }

                    var record = new FinancialCSV
                    {
                        Date = row[0],
                        Amount = amount,
                        Description = description
                    };
                    formattedRecords.Add(new FormattedFinancialCSV
                    {
                        Date = CsvFormatter.FormatDate(record.Date),
                        Description = CsvFormatter.FormatDescription(record.Description),
                        Amount = record.Amount
                    });
                }
            }

            //var records = csv.GetRecords<FinancialCSV>();
            //foreach (var record in records)
            //{
            //    formattedRecords.Add(new FormattedFinancialCSV()
            //    {
            //        Date = CsvFormatter.FormatDate(record.Date),
            //        Description = CsvFormatter.FormatDescription(record.Description),
            //        Amount = record.Amount
            //    });
            //}

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

                if (expenseTagsMatch is null && expenseRuleTagsMatch is null)
                    untaggedRecords.Add(record);
            }

            return [.. untaggedRecords.Distinct()];
        }
    }
}