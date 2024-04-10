using Core.Model;
using System.ComponentModel.DataAnnotations;

namespace API.DTO
{
    public class IncomeDTO
    {
        [Required]
        public double Amount { get; set; }
        public Frequency Frequency { get; set; }
        public string Source { get; set; }
        public List<string> Tags { get; set; }
    }
}
