using MongoDB.Bson;

namespace IBudget.Core.Model
{
    public class UserExpenseDictionary
    {
        public ObjectId _id { get; set; }
        public int userId { get; set; }
        public List<ExpenseDictionary> ExpenseDictionaries { get; set; }
    }
}
