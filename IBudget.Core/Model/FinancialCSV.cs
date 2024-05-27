using CsvHelper.Configuration.Attributes;

namespace IBudget.Core.Model
{
    public class FinancialCSV
    {
        [Index(0)]
        public string Date { get; set; }
        [Index(1)]
        public double Amount { get; set; }
        [Index(2)]
        public string Description { get; set; }
    }
}