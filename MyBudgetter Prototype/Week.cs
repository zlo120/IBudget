namespace MyBudgetter_Prototype
{
    public class Week
    {
        public Month Month { get; }
        public string Label { get; }
        public Week(int week, Month month, string label)
        {
            Month = month;   
            Label = label;
        }
    }
}