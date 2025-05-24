using Microsoft.AspNetCore.Mvc;
using RuinsandMuseums.Data;
using System.Linq;

namespace RuinsandMuseums.Controllers
{
    public class RuinsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public RuinsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var ruins = _context.Ruins.ToList();
            return View(ruins);
        }
    }
} 