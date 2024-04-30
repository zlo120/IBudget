using IBudget.Core.Model;
using System.ComponentModel.DataAnnotations;

namespace IBudget.API.DTO
{
    public class WeekDTO
    {
        [Required]
        public string WeekLabel { get; set; }
        [Required]
        public DateOnly Start { get; set; }
        [Required]
        public DateOnly End { get; set; }
        public List<IncomeDTO>? AllIncome { get; set; }
        public List<ExpenseDTO>? AllExpenses { get; set; }
    }
}