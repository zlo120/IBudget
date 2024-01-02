namespace MyBudgetter_Prototype.Model.Summaries
{
    public class YearlySummary : Summary
    {
        public Year Year { get; set; }
        public YearlySummary(Year year)
        {
            Year = year;
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
