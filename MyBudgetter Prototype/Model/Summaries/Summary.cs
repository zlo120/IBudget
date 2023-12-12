namespace MyBudgetter_Prototype.Model.Summaries
{
    public abstract class Summary : ISummary
    {
        public double Balance { get; set; }
        public double Equity { get; set; }
        public abstract double CalculateBalance();
        public abstract double CalculateEquity();
    }
}