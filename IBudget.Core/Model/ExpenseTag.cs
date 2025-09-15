using System.Diagnostics.CodeAnalysis;
using IBudget.Core.Utils;

namespace IBudget.Core.Model
{
    public class ExpenseTag : BaseModel, IEquatable<ExpenseTag>, IEqualityComparer<ExpenseTag>
    {
        public required string Title { get; set; }
        public required List<string> Tags { get; set; }
        public required DateTime CreatedAt { get; set; }
        public static bool operator ==(ExpenseTag eD1, ExpenseTag eD2) => eD1.Equals(eD2);
        public static bool operator !=(ExpenseTag eD1, ExpenseTag eD2) => !eD1.Equals(eD2);
        public override bool Equals(object obj) => Equals(obj as ExpenseTag);

        public bool Equals(ExpenseTag? other)
        {
            if (other is null) return false;
            if (Title.Equals(other.Title)) return true;
            return false;
        }
        public bool Equals(ExpenseTag? x, ExpenseTag? y)
        {
            if (x is null || y is null) return false;
            if (x.Title.Equals(y.Title)) return true;
            return false;
        }
        public int GetHashCode([DisallowNull] ExpenseTag obj) => (obj.Title, obj.Tags).GetHashCode();
        public override int GetHashCode() => (Title, Tags).GetHashCode();
        public override string ToString() => WriteObject.ToJsonString(this);
    }
}