using MongoDB.Bson;

namespace IBudget.Core.Model
{
    public class ExpenseDictionary
    {
        public string title { get; set; }
        public string[] tags { get; set; }
    }
}