using IBudget.Core.Model;

namespace IBudget.Core.RepositoryInterfaces
{
    public interface ISummaryRepository
    {
        Task<Week> ReadWeek(DateTime date);
        Task<Month> ReadMonth(int month);
    }
}
