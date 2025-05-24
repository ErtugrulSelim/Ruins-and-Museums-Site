using RuinsandMuseums.Models;

namespace RuinsandMuseums.Models.ViewModels
{
    public class ManageViewModel
    {
        public required IEnumerable<Ruin> Ruins { get; set; }
        public required IEnumerable<Museum> Museums { get; set; }
    }
} 