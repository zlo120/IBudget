using IBudget.API.DTO;
using IBudget.Core.Interfaces;
using IBudget.Core.Model;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace IBudget.API.Controllers
{
    public class CSVParserController : ControllerBase
    {
        private readonly ICSVParserService _csvParserService;

        public CSVParserController(ICSVParserService csvParserServce)
        {
            _csvParserService = csvParserServce;
        }

        [HttpPost("ParseCSV")]
        public async Task<IActionResult> ParseCSV()
        {
            // Parse the csv file
            // if the csv has untagged records
            //    return untagged records to user
            // else
            //    insert all records into the database
            //    return success
            throw new NotImplementedException();
        }

        [HttpPost("OrganiseCSV")]
        public async Task<IActionResult> OrganiseCSV([FromBody]CsvDTO[] csvData)
        {
            var formattedCSVs = new List<FormattedFinancialCSV>();
            foreach(var csv in csvData)
            {
                var formattedCsv = new FormattedFinancialCSV()
                {
                    Amount = csv.Amount,
                    Description = csv.Description,
                    Date = DateOnly.FromDateTime(DateTime.ParseExact(csv.Date, "MM/dd/yyyy", CultureInfo.InvariantCulture))
                };
                formattedCSVs.Add(formattedCsv);
            }
            var result = await _csvParserService.DistinguishTaggedAndUntagged(formattedCSVs);
            return Ok(new
            {
                UntaggedRecords = result.Item1,
                TaggedRecords = result.Item2
            });
        }
    }
}