using CsvHelper;
using CsvHelper.Configuration;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using IBudget.Core.Utils;
using System.Collections.Generic;
using System.Globalization;

namespace IBudget.Core.Services
{
    public class CSVParserService : ICSVParserService
    {
        private readonly IUserExpenseDictionaryService _userExpenseDictionaryService;

        public CSVParserService(IUserExpenseDictionaryService userExpenseDictionaryService)
        {
            _userExpenseDictionaryService = userExpenseDictionaryService;
        }
        public async Task<List<FormattedFinancialCSV>> ParseCSV(string csvFilePath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false
            };

            using (var reader = new StreamReader(csvFilePath)) 
            using (var csv = new CsvReader(reader, config))
            {
                var records = csv.GetRecords<FinancialCSV>();
                var formattedRecords = new List<FormattedFinancialCSV>();
                foreach(var record in records)
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
        public async Task<(List<FormattedFinancialCSV>, List<FormattedFinancialCSV>)> DistinguishTaggedAndUntagged(List<FormattedFinancialCSV> records)
        {
            records = records.Distinct().ToList();
            var userExpenseDictionaries = await _userExpenseDictionaryService.GetExpenseDictionary(1); // replace this with userId from config
            if (userExpenseDictionaries is null)
            {
                await _userExpenseDictionaryService.AddExpenseDictionary(new UserExpenseDictionary()
                {
                    userId = 1, // replace this with userId from config
                    RuleDictionary = new List<RuleDictionary>(),
                    ExpenseDictionaries = new List<ExpenseDictionary>()
                });

                userExpenseDictionaries = await _userExpenseDictionaryService.GetExpenseDictionary(1); // replace this with userId from config
            }
            var untaggedRecords = new List<FormattedFinancialCSV>();
            var taggedRecords = new List<FormattedFinancialCSV>();
            foreach (var record in records)
            {
                var expenseDictionaryMatch = userExpenseDictionaries.ExpenseDictionaries
                    .Where(expenseDictionary => expenseDictionary.title.Equals(record.Description))
                    .FirstOrDefault();

                var ruleDictionaryMatch = userExpenseDictionaries.RuleDictionary
                    .Where(ruleDictionary => 
                        record.Description.Contains(ruleDictionary.rule)
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