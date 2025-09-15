using IBudget.Core.Model;

namespace IBudget.Core.Interfaces
{
    public interface ICSVParserService
    {
        /// <summary>
        /// Parses through CSV file and formats the data into FormattedFinancialCSV objects
        /// </summary>
        /// <param name="csvFilePath"></param>
        /// <returns>List of FormattedFinancialCSV</returns>
        public List<FormattedFinancialCSV> ParseCSV(string csvFilePath);

        /// <summary>
        /// Finds untagged ExpenseDictionaries
        /// </summary>
        /// <param name="records"></param>
        /// <returns>List of Formatted Financial Csv</returns>
        public Task<List<FormattedFinancialCSV>> FindUntagged(List<FormattedFinancialCSV> records);
    }
}