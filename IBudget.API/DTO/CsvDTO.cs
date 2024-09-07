using System.ComponentModel.DataAnnotations;

namespace IBudget.API.DTO
{
    public class CsvDTO
    {
        public string? Date { get; set; }
        public string? Description { get; set; }
        public double? Amount { get; set; }
    }
}
