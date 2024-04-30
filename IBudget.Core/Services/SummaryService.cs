using IBudget.Core.Interfaces;
using IBudget.Core.Model;

namespace IBudget.Core.Services
{
    public class SummaryService : ISummaryService
    {
        private readonly ISummaryRepository _summaryRepository;
        private readonly ICalendarService _calendarService;

        public SummaryService(ISummaryRepository summaryRepository, ICalendarService calendarService)
        {
            _summaryRepository = summaryRepository;
            _calendarService = calendarService;
        }
        public async Task<Week> ReadWeek(DateTime date)
        {
            var week = await _summaryRepository.ReadWeek(date);
            return await _calendarService.RetrieveWeekData(week);            
        }
        public async Task<Month> ReadMonth(int monthNum)
        {
            var month = await _summaryRepository.ReadMonth(monthNum);
            return await _calendarService.RetrieveMonthData(month);
        }

    }
}
