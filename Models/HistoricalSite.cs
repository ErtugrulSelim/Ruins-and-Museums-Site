using System.ComponentModel.DataAnnotations;

namespace RuinsandMuseums.Models
{
    public abstract class HistoricalSite
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;
    }
} 