using System.ComponentModel.DataAnnotations;

namespace IBudget.API.DTO
{
    public class CsvUploadBatch
    {
        [Required]
        public CsvDTO[] CsvBatch { get; set; }
        [Required]
        public string BatchHash { get; set; }
    }
}
