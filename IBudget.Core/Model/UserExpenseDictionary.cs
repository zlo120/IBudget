using MongoDB.Bson;

namespace IBudget.Core.Model
{
    public class UserExpenseDictionary
    {
        public ObjectId _id { get; set; }
        public int userId { get; set; }
        public List<ExpenseDictionary> ExpenseDictionaries { get; set; }
        public bool Equals(UserExpenseDictionary? other)
        {
            if (other is null) return false;

            if (userId != other.userId) return false;

            return true;
        }
        public static bool operator ==(UserExpenseDictionary eD1, UserExpenseDictionary eD2) => eD1.Equals(eD2);
        public static bool operator !=(UserExpenseDictionary eD1, UserExpenseDictionary eD2) => !eD1.Equals(eD2);
        public override bool Equals(object obj) => Equals(obj as UserExpenseDictionary);
        public override int GetHashCode() => (_id, userId, ExpenseDictionaries).GetHashCode();
    }
}
