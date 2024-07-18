using IBudget.Core.Model;
using System.ComponentModel.DataAnnotations;

namespace IBudget.API.DTO
{
    public class IncomeDTO
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public double Amount { get; set; }
        [Required]
        public string? Date { get; set; }
        [Required]
        public string Description { get; set; }
        public List<string>? Tags { get; set; }

    }
}
