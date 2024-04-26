using IBudget.Core.Model;

namespace IBudget.Core.Interfaces
{
    public interface ISummaryService
    {
        Task<Week> ReadWeek(DateTime date);
        Task<Month> ReadMonth(int month);
    }
}
