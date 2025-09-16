using CsvHelper.Configuration;
using IBudget.Core.Model;

namespace IBudget.Core.Utils
{
    public class DynamicFinancialCSVMap : ClassMap<FinancialCSV>
    {
        public DynamicFinancialCSVMap(Dictionary<string, string> userHeaderToPropertyMap)
        {
            foreach (var kvp in userHeaderToPropertyMap)
            {
                Map(typeof(FinancialCSV), typeof(FinancialCSV).GetProperty(kvp.Value)).Name(kvp.Key);
            }
        }
    }
}
