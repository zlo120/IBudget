using CsvHelper;
using CsvHelper.Configuration;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using IBudget.Core.Utils;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace IBudget.Core.Services
{
    public class CSVParserService : ICSVParserService
    {
        private readonly IUserDictionaryService? _userDictionaryService;
        private readonly int? _userId;
        public CSVParserService(IUserDictionaryService userDictionaryService)
        {
            _userDictionaryService = userDictionaryService;
        }
        public CSVParserService(IUserDictionaryService userDictionaryService, IConfiguration config)
        {
            _userDictionaryService = userDictionaryService;
            _userId = int.Parse(config["MongoDbUserId"]!);
        }
        public async Task<List<FormattedFinancialCSV>> ParseCSV(string csvFilePath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            };

            using (var reader = new StreamReader(csvFilePath))
            using (var csv = new CsvReader(reader, config))
            {
                var records = csv.GetRecords<FinancialCSV>();
                var formattedRecords = new List<FormattedFinancialCSV>();
                foreach (var record in records)
                {
                    if (record.Amount >= 0) continue;
                    formattedRecords.Add(new FormattedFinancialCSV()
                    {
                        Date = CsvFormatter.FormatDate(record.Date),
                        Description = CsvFormatter.FormatDescription(record.Description),
                        Amount = record.Amount
                    });
                }
                return formattedRecords;
            }
        }
        public async Task<List<FormattedFinancialCSV>> FindUntagged(List<FormattedFinancialCSV> records)
        {
            records = records.Distinct().ToList();
            List<ExpenseDictionary>? expenseDictionaries = await _userDictionaryService!.GetExpenseDictionaries(_userId ?? -1) ?? new List<ExpenseDictionary>();
            List<RuleDictionary>? ruleDictionaries = await _userDictionaryService.GetRuleDictionaries(_userId ?? -1) ?? new List<RuleDictionary>();

            var untaggedRecords = new List<FormattedFinancialCSV>();
            foreach (var record in records)
            {
                var expenseDictionaryMatch = expenseDictionaries
                    .Where(expenseDictionary => expenseDictionary.title.Equals(record.Description, StringComparison.InvariantCultureIgnoreCase))
                    .FirstOrDefault();

                var ruleDictionaryMatch = ruleDictionaries
                    .Where(ruleDictionary =>
                        record.Description.Contains(ruleDictionary.rule, StringComparison.InvariantCultureIgnoreCase)
                        )
                    .FirstOrDefault();

                if (expenseDictionaryMatch is null && ruleDictionaryMatch is null)
                    untaggedRecords.Add(record);
            }

            return untaggedRecords.Distinct().ToList();
        }
    }
}