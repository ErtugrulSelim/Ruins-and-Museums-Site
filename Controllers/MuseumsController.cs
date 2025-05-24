using Microsoft.AspNetCore.Mvc;
using RuinsandMuseums.Data;
using System.Linq;

namespace RuinsandMuseums.Controllers
{
    public class MuseumsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public MuseumsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var museums = _context.Museums.ToList();
            return View(museums);
        }
    }
} 