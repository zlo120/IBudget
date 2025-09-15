using System.Diagnostics.CodeAnalysis;
using IBudget.Core.Utils;

namespace IBudget.Core.Model
{
    public class ExpenseRuleTag : BaseModel
    {
        public required string Rule { get; set; }
        public required List<string> Tags { get; set; }
        public required DateTime CreatedAt { get; set; }
        public static bool operator ==(ExpenseRuleTag rD1, ExpenseRuleTag rD2) => rD1.Equals(rD2);
        public static bool operator !=(ExpenseRuleTag rD1, ExpenseRuleTag rD2) => !rD1.Equals(rD2);
        public override bool Equals(object obj) => Equals(obj as ExpenseRuleTag);
        public bool Equals(ExpenseRuleTag? other)
        {
            if (other is null) return false;
            if (Rule.Equals(other.Rule)) return true;
            return false;
        }
        public bool Equals(ExpenseRuleTag? x, ExpenseRuleTag? y)
        {
            if (x is null || y is null) return false;
            if (x.Rule.Equals(y.Rule)) return true;
            return false;
        }
        public int GetHashCode([DisallowNull] ExpenseRuleTag obj) => (obj.Rule, obj.Tags).GetHashCode();
        public override int GetHashCode() => (Rule, Tags).GetHashCode();
        public override string ToString() => WriteObject.ToJsonString(this);
    }
}
