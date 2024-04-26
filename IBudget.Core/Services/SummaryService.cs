using IBudget.Core.Interfaces;
using IBudget.Core.Model;

namespace IBudget.Core.Services
{
    public class SummaryService : ISummaryService
    {
        private readonly ISummaryRepository _summaryRepository;
        public SummaryService(ISummaryRepository summaryRepository)
        {
            _summaryRepository = summaryRepository;
        }
        public async Task<Week> ReadWeek(DateTime date)
        {
            return await _summaryRepository.ReadWeek(date);
        }
        public async Task<Month> ReadMonth(int month)
        {
            return await _summaryRepository.ReadMonth(month);
        }

    }
}
