namespace MyBudgetter_Prototype.Model.Summaries
{
    public class MonthlySummary : Summary
    {
        public Month Month { get; set; }
        public MonthlySummary(Month month)
        {
            Month = month;
        }

        public override double CalculateBalance()
        {
            throw new NotImplementedException();
        }

        public override double CalculateEquity()
        {
            throw new NotImplementedException();
        }
    }
}
