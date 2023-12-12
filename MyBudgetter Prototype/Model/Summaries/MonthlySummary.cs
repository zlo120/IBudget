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
            double balance = 0;
            foreach (var week in Month.Weeks)
            {
                foreach (var income in week.Income)
                {
                    balance += income.Amount;
                }

                foreach (var expense in week.Expenses)
                {
                    balance -= expense.Amount;
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
