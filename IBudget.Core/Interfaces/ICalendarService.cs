using IBudget.Core.DTO;

namespace IBudget.Core.Interfaces
{
    public interface ICalendarService
    {
        Task<MonthDTO> RetrieveMonthData(MonthDTO month);
        Task<WeekDTO> RetrieveWeekData(WeekDTO week);
    }
}
