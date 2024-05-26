using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace IBudget.Core.Model
{
    public class ExpenseDictionary : IEquatable<ExpenseDictionary>, IEqualityComparer<ExpenseDictionary>
    {
        public string title { get; set; }
        public string[] tags { get; set; }
        public static bool operator ==(ExpenseDictionary eD1, ExpenseDictionary eD2) => eD1.Equals(eD2);
        public static bool operator !=(ExpenseDictionary eD1, ExpenseDictionary eD2) => !eD1.Equals(eD2);
        public override bool Equals(object obj) => Equals(obj as ExpenseDictionary);
        public bool Equals(ExpenseDictionary? other)
        {
            if (other is null) return false;
            if (title.Equals(other.title)) return true;
            return false;
        }
        public bool Equals(ExpenseDictionary? x, ExpenseDictionary? y)
        {
            if (x is null || y is null) return false;
            if (x.title.Equals(y.title)) return true;
            return false;
        }
        public int GetHashCode([DisallowNull] ExpenseDictionary obj) => (obj.title, obj.tags).GetHashCode();
        public override int GetHashCode() => (title, tags).GetHashCode();
    }
}