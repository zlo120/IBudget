namespace Core.Model
{
    public abstract class DataEntry 
    {
        public int? ID { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }
        public double Amount { get; set; }
    }
}