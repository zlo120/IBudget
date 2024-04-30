using IBudget.Core.Model;
using System.ComponentModel.DataAnnotations;

namespace IBudget.API.DTO
{
    public class MonthDTO
    {
        [Required]
        public string MonthName { get; set; }
        public List<IncomeDTO>? TotalIncome { get; set; }
        public List<ExpenseDTO>? TotalExpenses { get; set; }
    }
}