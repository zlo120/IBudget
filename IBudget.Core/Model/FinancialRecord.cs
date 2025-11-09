using IBudget.Core.Utils;

namespace IBudget.Core.Model
{
    public abstract class FinancialRecord : BaseModel
    {
        public required DateTime Date { get; set; }
        public required double Amount { get; set; }
        public Frequency? Frequency { get; set; }
        public required List<Tag> Tags { get; set; }
        public required string BatchHash { get; set; }
        public required DateTime CreatedAt { get; set; }
        public override string ToString() => WriteObject.ToJsonString(this);
        public required bool IsIgnored { get; set; }
    }
}