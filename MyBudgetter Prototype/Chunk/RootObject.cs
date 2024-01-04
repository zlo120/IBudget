using MyBudgetter_Prototype.Model;

namespace MyBudgetter_Prototype.Chunk
{
    public class RootObject
    {
        public List<DateTime> DateRange { get; set; }
        public List<Tag> Tags { get; set; }
        public List<Income> IncomeRecords { get; set; }
        public List<Expense> ExpenseRecords { get; set; }
    }
}