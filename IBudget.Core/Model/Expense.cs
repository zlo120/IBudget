using IBudget.Core.Utils;

namespace IBudget.Core.Model
{
    public class Expense : FinancialRecord
    {
        public string? Notes { get; set; }
        public override string ToString() => WriteObject.ToJsonString(this);
    }
}