namespace MyBudgetter_Prototype.Model
{
    public class Week
    {
        public Month Month { get; }
        public string Label { get; }
        public List<Expense> Expenses { get; set; }
        public List<Income> Income { get; set; }
        public Week(int week, Month month, string label)
        {
            Month = month;
            Label = label;
        }
    }
}