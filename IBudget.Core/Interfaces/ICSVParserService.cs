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
        public Task<List<FormattedFinancialCSV>> ParseCSV(string csvFilePath);

        /// <summary>
        /// Seperates tagged and untagged ExpenseDictionaries
        /// </summary>
        /// <param name="records"></param>
        /// <returns>Tuple where Item1 is untaggedRecords and Item2 is tagged records: (untaggedRecords, taggedRecords)</returns>
        public Task<(List<FormattedFinancialCSV>, List<FormattedFinancialCSV>)> DistinguishTaggedAndUntagged(List<FormattedFinancialCSV> records);
    }
}