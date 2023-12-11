namespace MyBudgetter_Prototype.Model
{
    public abstract class DataEntry 
    {
        public string Category { get; set; }
        public DateTime Date { get; set; }
        public double Amount { get; set; }
    }
}