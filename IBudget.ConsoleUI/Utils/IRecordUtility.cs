using IBudget.Core.Model;

namespace IBudget.ConsoleUI.Utils
{
    public interface IRecordUtility
    {
        Task<FinancialRecord> FindRecord();
    }
}