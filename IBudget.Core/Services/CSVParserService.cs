using IBudget.Core.Interfaces;
using IBudget.Core.Model;

namespace IBudget.Core.Services
{
    public class CSVParserService : ICSVParserService
    {
        private readonly IUserExpenseDictionaryService _userExpenseDictionaryService;

        public CSVParserService(IUserExpenseDictionaryService userExpenseDictionaryService)
        {
            _userExpenseDictionaryService = userExpenseDictionaryService;
        }

        public Task ParseCSV(string csvFilePath)
        {
            throw new NotImplementedException();
        }
    }
}