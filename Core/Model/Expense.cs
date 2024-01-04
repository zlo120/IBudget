namespace Core.Model
{
    public class Expense : DataEntry
    {
        public Frequency? Frequency { get; set; }
        public string? Notes { get; set; }
        public List<string>? Tags { get; set; }
        public List<int>? TagIDs { get; set; }
    }
}