using IBudget.Core.DTO;
using IBudget.Core.Interfaces;
using IBudget.Core.RepositoryInterfaces;

namespace IBudget.Core.Services
{
    public class SummaryService : ISummaryService
    {

        public Task<MonthDTO> ReadMonth(int month)
        {
            throw new NotImplementedException();
        }

        public Task<WeekDTO> ReadWeek(DateTime date)
        {
            throw new NotImplementedException();
        }
    }
}
