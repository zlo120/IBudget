using Core.Model;

namespace Core.Chunk
{
    public class RootObject
    {
        public List<DateTime> DateRange { get; set; }
        public List<Tag> Tags { get; set; }
        public List<Income> IncomeRecords { get; set; }
        public List<Expense> ExpenseRecords { get; set; }
        public RootObject() {
            DateRange = new List<DateTime>();
            Tags = new List<Tag>();
            ExpenseRecords = new List<Expense>();
            IncomeRecords = new List<Income>();
        }
    }
}