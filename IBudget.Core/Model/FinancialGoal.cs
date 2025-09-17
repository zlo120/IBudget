using IBudget.Core.Utils;

namespace IBudget.Core.Model
{
    public class FinancialGoal : BaseModel
    {
        public required string Name { get; set; }
        public required decimal TargetAmount { get; set; }
        public required DateTime CreatedAt { get; set; }
        public override bool Equals(object? obj)
        {
            if (obj is null || obj is not Tag) return false;
            var other = obj as Tag;
            if (other?.Name == Name) return true;
            return false;
        }

        public int GetHashCode(FinancialGoal obj) => (obj.Name, obj.TargetAmount).GetHashCode();
        public override int GetHashCode() => (Name, TargetAmount).GetHashCode();
        public override string ToString() => WriteObject.ToJsonString(this);
    }
}
