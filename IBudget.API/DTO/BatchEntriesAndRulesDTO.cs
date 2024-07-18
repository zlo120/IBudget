using System.ComponentModel.DataAnnotations;

namespace IBudget.API.DTO
{
    public class EntriesDTO
    {
        [Required]
        public string Captures { get; set; }
        [Required]
        public string[] Tags { get; set; }
    }
    public class RulesDTO
    {
        [Required]
        public string Rule { get; set; }
        [Required]
        public string[] Tags { get; set; }
    }
    public class BatchEntriesAndRulesDTO
    {
        [Required]
        public List<EntriesDTO> Entries { get; set; }
        [Required]
        public List<RulesDTO> Rules { get; set; }
    }
}
