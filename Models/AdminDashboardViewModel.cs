using System.Collections.Generic;

namespace RuinsandMuseums.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalRuins { get; set; }
        public int TotalMuseums { get; set; }
        public List<User> RecentUsers { get; set; } = new List<User>();
        public List<Ruin> RecentRuins { get; set; } = new List<Ruin>();
        public List<Museum> RecentMuseums { get; set; } = new List<Museum>();
    }
} 