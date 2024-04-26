using IBudget.Core.Model;

namespace IBudget.Core.Interfaces
{
    public interface ISummaryRepository
    {
        Task<Week> ReadWeek(DateTime date);
        Task<Month> ReadMonth(int month);
    }
}
