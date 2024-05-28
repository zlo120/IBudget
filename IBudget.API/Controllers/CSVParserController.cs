using IBudget.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IBudget.API.Controllers
{
    public class CSVParserController
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
    }
}