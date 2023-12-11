namespace MyBudgetter_Prototype.Model
{
    public class Year
    {
        public int YearNumber { get; }
        public List<Month> Months { get; set; }
        public Year(int year)
        {
            YearNumber = year;
        }
    }
}