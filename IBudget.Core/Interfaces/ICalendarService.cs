using IBudget.Core.Model;

namespace IBudget.Core.Interfaces
{
    public interface ICalendarService
    {
        Task<Month> RetrieveMonthData(Month month);
        Task<Week> RetrieveWeekData(Week week);
    }
}
