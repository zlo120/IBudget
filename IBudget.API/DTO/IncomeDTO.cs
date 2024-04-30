using IBudget.Core.Model;
using System.ComponentModel.DataAnnotations;

namespace IBudget.API.DTO
{
    public class IncomeDTO
    {
        [Required]
        public double Amount { get; set; }
        [Required]
        public DateOnly? Date { get; set; }
        public Frequency? Frequency { get; set; }
        public string? Source { get; set; }
        public List<string>? Tags { get; set; }
    }
}
