namespace IBudget.Core.Model
{
    public class FormattedFinancialCSV
    {
        public DateOnly Date { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    }
}