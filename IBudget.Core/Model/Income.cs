using IBudget.Core.Utils;

namespace IBudget.Core.Model
{
    public class Income : FinancialRecord
    {
        public string? Source { get; set; }
        public override string ToString() => WriteObject.ToJsonString(this);
    }
}
