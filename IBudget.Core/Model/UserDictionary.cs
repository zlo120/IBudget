using MongoDB.Bson;

namespace IBudget.Core.Model
{
    public class UserDictionary
    {
        public ObjectId _id { get; set; }
        public int userId { get; set; }
        public List<ExpenseDictionary>? ExpenseDictionaries { get; set; }
        public List<RuleDictionary>? RuleDictionaries { get; set; }
        public List<string>? BatchHashes { get; set; }
        public bool Equals(UserDictionary? other)
        {
            if (other is null) return false;

            if (userId != other.userId) return false;

            return true;
        }
        public static bool operator ==(UserDictionary eD1, UserDictionary eD2) => eD1.Equals(eD2);
        public static bool operator !=(UserDictionary eD1, UserDictionary eD2) => !eD1.Equals(eD2);
        public override bool Equals(object obj) => Equals(obj as UserDictionary);
        public override int GetHashCode() => (_id, userId, ExpenseDictionaries).GetHashCode();
        public override string ToString()
        {
            var output = $"{{\n    _id: {_id},\n    userId: {userId},\n\texpenseDictionaries: {{";
            if (ExpenseDictionaries.Count == 0) output += "},\n";
            foreach(var expense in ExpenseDictionaries)
                output += $"\n        {expense}";
            if (ExpenseDictionaries.Count > 0)
                output += "\n    }";

            output += "\n    ruleDictionary: {";
            if (RuleDictionaries.Count == 0) output += "}\n";
            foreach (var rule in RuleDictionaries)
                output += $"\n        {rule}";
            if (RuleDictionaries.Count > 0)
                output += "\n    }";

            output += "\n}";
            return output;
        }
    }
}
