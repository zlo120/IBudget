namespace MyBudgetter_Prototype.Model.Summaries
{
    public class WeeklySummary : Summary
    {
        public Week Week { get; set; }
        public WeeklySummary(Week week)
        {
            Week = week;
        }

        public override double CalculateBalance()
        {
            double balance = 0;

            foreach (var income in Week.Income)
            {
                balance += income.Amount;
            }

            foreach (var expense in Week.Expenses)
            {
                balance -= expense.Amount;
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
