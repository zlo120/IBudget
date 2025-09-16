using CsvHelper.Configuration;
using IBudget.Core.Model;

namespace IBudget.Core.Utils
{
    public class FinancialCSVNoHeaderMap : ClassMap<FinancialCSV>
    {
        public FinancialCSVNoHeaderMap()
        {
            Map(m => m.Date).Index(0);         // First column
            Map(m => m.Amount).Index(1);       // Second column
            Map(m => m.Description).Index(2);  // Third column
                                               // Add more mappings if your class has more properties
        }
    }
}
