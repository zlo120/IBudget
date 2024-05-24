namespace IBudget.Core.Model
{
    public class ExpenseDictionary : IEquatable<ExpenseDictionary>
    {
        public string title { get; set; }
        public string[] tags { get; set; }
        public bool Equals(ExpenseDictionary? other)
        {
            if (other is null) return false;

            if (title != other.title) return false;

            return true;
        }
        public static bool operator ==(ExpenseDictionary eD1, ExpenseDictionary eD2) => eD1.Equals(eD2);
        public static bool operator !=(ExpenseDictionary eD1, ExpenseDictionary eD2) => !eD1.Equals(eD2);
        public override bool Equals(object obj) => Equals(obj as ExpenseDictionary);
        public override int GetHashCode() => (title, tags).GetHashCode();
    }
}