using System.ComponentModel.DataAnnotations;

namespace RuinsandMuseums.Models.ViewModels
{
    public class RuinsAndMuseumsViewModel
    {
        public Ruin NewRuin { get; set; } = new Ruin();
        public Museum NewMuseum { get; set; } = new Museum();
        public List<Ruin> Ruins { get; set; } = new List<Ruin>();
        public List<Museum> Museums { get; set; } = new List<Museum>();
    }
} 