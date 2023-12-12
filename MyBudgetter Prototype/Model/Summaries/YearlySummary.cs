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
            // Brute force method
            double balance = 0;
            foreach(var month in Year.Months)
            {
                foreach (var week in month.Weeks)
                {
                    foreach (var income in week.Income)
                    {
                        balance += income.Amount;
                    }

                    foreach(var expense in week.Expenses)
                    {
                        balance -= expense.Amount;
                    }
                }
            }
            base.Balance = balance;
            return balance;
        }

        public override double CalculateEquity()
        {
            throw new NotImplementedException();
        }
    }
}
