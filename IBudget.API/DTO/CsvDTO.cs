using System.ComponentModel.DataAnnotations;

namespace IBudget.API.DTO
{
    public class CsvDTO
    {
        [Required]
        public string Date { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public double Amount { get; set; }
    }
}
