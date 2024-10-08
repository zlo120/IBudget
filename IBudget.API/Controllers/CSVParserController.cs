﻿using IBudget.API.DTO;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using IBudget.Core.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace IBudget.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CSVParserController : ControllerBase
    {
        private readonly ICSVParserService _csvParserService;
        private readonly IUserDictionaryService _userDictionaryService;
        private readonly ITagService _tagService;
        private readonly IIncomeService _incomeService;
        private readonly IExpenseService _expenseService;
        private readonly IConfiguration _config;

        public CSVParserController(ICSVParserService csvParserServce, 
            IUserDictionaryService userDictionaryService, 
            ITagService tagService,
            IIncomeService incomeService,
            IExpenseService expenseService,
            IConfiguration config)
        {
            _csvParserService = csvParserServce;
            _userDictionaryService = userDictionaryService;
            _tagService = tagService;
            _incomeService = incomeService;
            _expenseService = expenseService;
            _config = config;
        }

        [HttpPost("ParseCSV")]
        public async Task<IActionResult> ParseCSV([FromBody] CsvUploadBatch csvBatch)
        {
            // verify hash doesn't exist
            var user = await _userDictionaryService.GetUser(int.Parse(_config["MongoDbUserId"]));
            if (user.BatchHashes.Contains(csvBatch.BatchHash))
                return BadRequest("This batch has already been uploaded!");

            // receive all the data without tags,
            // then tag all the data
            foreach (var csv in csvBatch.CsvBatch)
            {
                if (csv.Description is null)
                    continue;
                csv.Description = CsvFormatter.FormatDescription(csv.Description!);
                var tags = await _tagService.FindTagByDescription(csv.Description!);
                var formattedTags = new List<Tag>();
                foreach(var tag in tags)
                    formattedTags.Add(new Tag { Name = tag });

                if (csv.Amount > 0)
                {
                    var income = new Income()
                    {
                        Amount = (double) csv.Amount,
                        Source = csv.Description,
                        Date = DateTime.ParseExact(csv.Date!, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        Tags = formattedTags
                    };
                    await _incomeService.AddIncome(income);
                }
                else
                {
                    var expense = new Expense()
                    {
                        Amount = (double) csv.Amount!,
                        Notes = csv.Description,
                        Date = DateTime.ParseExact(csv.Date!, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        Tags = formattedTags
                    };
                    await _expenseService.AddExpense(expense);
                }
            }

            await _userDictionaryService.CreateBatchHash(int.Parse(_config["MongoDbUserId"]!), csvBatch.BatchHash);
            return Ok("All data has been tagged and inserted into the db!");
        }

        [HttpPost("BatchCreateNewEntriesAndRules")]
        public async Task<IActionResult> BatchCreateNewEntriesAndRules([FromBody] BatchEntriesAndRulesDTO batchEntriesAndRules)
        {
            var ruleDictionary = await _userDictionaryService.GetRuleDictionaries(int.Parse(_config["MongoDbUserId"]!));
            var allDuplicateRules = batchEntriesAndRules.Rules.Where(r => ruleDictionary.Any(rd => r.Rule == rd.rule)).ToList();
            foreach (var duplicates in allDuplicateRules) 
                batchEntriesAndRules.Rules.Remove(duplicates);

            foreach(var rule in batchEntriesAndRules.Rules)
            {
                var rd = new RuleDictionary()
                {
                    rule = rule.Rule,
                    tags = rule.Tags
                };
                await _userDictionaryService.AddRuleDictionary(int.Parse(_config["MongoDbUserId"]!), rd);
            }

            foreach(var entry in batchEntriesAndRules.Entries)
            {
                var ed = new ExpenseDictionary()
                {
                    title = entry.Captures,
                    tags = entry.Tags
                };
                await _userDictionaryService.AddExpenseDictionary(int.Parse(_config["MongoDbUserId"]!), ed);
            }
            return Ok();
        }

        [HttpPost("CreateNewEntry")]
        public async Task<IActionResult> CreateNewEntry([FromBody] EntriesDTO entryDto)
        {
            var expenseDictionaries = await _userDictionaryService.GetExpenseDictionaries(int.Parse(_config["MongoDbUserId"]!));
            if (expenseDictionaries.Any(eD => eD.title.Equals(entryDto.Captures, StringComparison.InvariantCultureIgnoreCase)))
                return BadRequest("This entry already exists");
            var expenseDictionary = new ExpenseDictionary()
            {
                title = entryDto.Captures,
                tags = entryDto.Tags
            };
            await _userDictionaryService.AddExpenseDictionary(int.Parse(_config["MongoDbUserId"]!), expenseDictionary);
            return Ok();
        }
        [HttpPost("CreateNewRule")]
        public async Task<IActionResult> CreateNewRule([FromBody] RulesDTO ruleDto)
        {
            var ruleDictionary = await _userDictionaryService.GetRuleDictionaries(int.Parse(_config["MongoDbUserId"]!));
            if (ruleDictionary.Any(rD => rD.rule == ruleDto.Rule))
                return BadRequest("That rule already exists");

            var rule = new RuleDictionary()
            {
                rule = ruleDto.Rule,
                tags = ruleDto.Tags
            };
            await _userDictionaryService.AddRuleDictionary(int.Parse(_config["MongoDbUserId"]!), rule);
            return Ok();
        }

        [HttpPost("FindUntagged")]
        public async Task<IActionResult> FindUntagged([FromBody]CsvDTO[] csvData)
        {
            var formattedCSVs = new List<FormattedFinancialCSV>();
            foreach(var csv in csvData)
            {
                if (csv.Amount is null || csv.Date is null)
                {
                    continue;
                }
                var formattedCsv = new FormattedFinancialCSV()
                {
                    Amount = (double) csv.Amount,
                    Description = CsvFormatter.FormatDescription(csv.Description ?? "No description."),
                    Date = DateOnly.FromDateTime(DateTime.ParseExact(csv.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture))
                };
                formattedCSVs.Add(formattedCsv);
            }
            var untagged = await _csvParserService.FindUntagged(formattedCSVs);
            return Ok(untagged);
        }
    }
}