using IBudget.Core.DTO;

namespace IBudget.Core.Interfaces
{
    public interface ISummaryService
    {
        Task<WeekDTO> ReadWeek(DateTime date, bool getOffset = false);
        Task<MonthDTO> ReadMonth(int month, bool getOffset = false);
    }
}
