using IBudget.Core.Model;

namespace IBudget.Core.Interfaces
{
    public interface ICSVParserService
    {
        public Task ParseCSV(string csvFilePath);
    }
}
