namespace IBudget.Core.DTO
{
    public class YearDTO
    {
        public int YearNumber { get; }
        public List<MonthDTO> Months { get; set; }
        public YearDTO(int year)
        {
            YearNumber = year;
            Months = new List<MonthDTO>();
        }
    }
}