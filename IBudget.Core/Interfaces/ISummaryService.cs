using IBudget.Core.DTO;

namespace IBudget.Core.Interfaces
{
    public interface ISummaryService
    {
        Task<WeekDTO> ReadWeek(DateTime date);
        Task<MonthDTO> ReadMonth(int month);
    }
}
