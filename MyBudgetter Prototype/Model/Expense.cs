namespace MyBudgetter_Prototype.Model
{
    public class Expense : DataEntry
    {
        public Frequency? Frequency { get; set; }
        public bool? Recurring { get; set; }
        public string? Notes { get; set; }
        public List<string>? Tags { get; set; }
    }
}