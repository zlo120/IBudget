using Core.Model;

namespace Core.Interfaces
{
    public interface ISummaryRepository
    {
        Task<Week> ReadWeek(DateTime date);
        Task<Month> ReadMonth(int month);
    }
}
