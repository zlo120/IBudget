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
        private readonly IUserDictionaryService _userDictionaryService;
        private readonly int _userId;

        public CSVParserService(IUserDictionaryService userDictionaryService, IConfiguration config)
        {
            _userDictionaryService = userDictionaryService;
            _userId = int.Parse(config["MongoDbUserId"]);
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
                        Amount = record.Amount * -1
                    });
                }

                return formattedRecords;
            }
        }
        public async Task<(List<FormattedFinancialCSV>, List<FormattedFinancialCSV>)> DistinguishTaggedAndUntagged(List<FormattedFinancialCSV> records)
        {
            records = records.Distinct().ToList();
            var userDictionary = await _userDictionaryService.GetUser(_userId);
            var untaggedRecords = new List<FormattedFinancialCSV>();
            var taggedRecords = new List<FormattedFinancialCSV>();
            foreach (var record in records)
            {
                var expenseDictionaryMatch = userDictionary.ExpenseDictionaries
                    .Where(expenseDictionary => expenseDictionary.title.Equals(record.Description, StringComparison.InvariantCultureIgnoreCase))
                    .FirstOrDefault();

                var ruleDictionaryMatch = userDictionary.RuleDictionaries
                    .Where(ruleDictionary =>
                        record.Description.Contains(ruleDictionary.rule, StringComparison.InvariantCultureIgnoreCase)
                        )
                    .FirstOrDefault();

                if (expenseDictionaryMatch is null && ruleDictionaryMatch is null)
                    untaggedRecords.Add(record);

                if (expenseDictionaryMatch is not null)
                {
                    record.Tags.AddRange(expenseDictionaryMatch.tags.ToList());
                    taggedRecords.Add(record);
                }

                if (ruleDictionaryMatch is not null)
                {
                    record.Tags.AddRange(ruleDictionaryMatch.tags.ToList());
                    taggedRecords.Add(record);
                }
            }

            return (untaggedRecords, taggedRecords);
        }
    }
}