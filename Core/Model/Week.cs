namespace Core.Model
{
    public class Week
    {
        public string Label { get; }
        public List<Expense> Expenses { get; set; }
        public List<Income> Income { get; set; }
        public Week(string label)
        {
            Label = label;

            Income = new List<Income>();
            Expenses = new List<Expense>();
        }
    }
}