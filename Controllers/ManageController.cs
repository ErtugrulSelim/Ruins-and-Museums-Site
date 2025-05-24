using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RuinsandMuseums.Models;
using RuinsandMuseums.Models.ViewModels;
using RuinsandMuseums.Data;
using Microsoft.EntityFrameworkCore;

namespace RuinsandMuseums.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManageController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var ruins = await _context.Ruins.ToListAsync();
            var museums = await _context.Museums.ToListAsync();
            
            var viewModel = new ManageViewModel
            {
                Ruins = ruins,
                Museums = museums
            };
            
            return View(viewModel);
        }

        // Ruin Management
        public IActionResult CreateRuin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRuin(Ruin ruin)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ruin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ruin);
        }

        public async Task<IActionResult> EditRuin(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ruin = await _context.Ruins.FindAsync(id);
            if (ruin == null)
            {
                return NotFound();
            }
            return View(ruin);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRuin(int id, Ruin ruin)
        {
            if (id != ruin.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ruin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RuinExists(ruin.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ruin);
        }

        public async Task<IActionResult> DeleteRuin(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ruin = await _context.Ruins
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ruin == null)
            {
                return NotFound();
            }

            return View(ruin);
        }

        [HttpPost, ActionName("DeleteRuin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRuinConfirmed(int id)
        {
            var ruin = await _context.Ruins.FindAsync(id);
            if (ruin != null)
            {
                _context.Ruins.Remove(ruin);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Museum Management
        public IActionResult CreateMuseum()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMuseum(Museum museum)
        {
            if (ModelState.IsValid)
            {
                _context.Add(museum);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(museum);
        }

        public async Task<IActionResult> EditMuseum(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var museum = await _context.Museums.FindAsync(id);
            if (museum == null)
            {
                return NotFound();
            }
            return View(museum);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMuseum(int id, Museum museum)
        {
            if (id != museum.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(museum);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MuseumExists(museum.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(museum);
        }

        public async Task<IActionResult> DeleteMuseum(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var museum = await _context.Museums
                .FirstOrDefaultAsync(m => m.Id == id);
            if (museum == null)
            {
                return NotFound();
            }

            return View(museum);
        }

        [HttpPost, ActionName("DeleteMuseum")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMuseumConfirmed(int id)
        {
            var museum = await _context.Museums.FindAsync(id);
            if (museum != null)
            {
                _context.Museums.Remove(museum);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool RuinExists(int id)
        {
            return _context.Ruins.Any(e => e.Id == id);
        }

        private bool MuseumExists(int id)
        {
            return _context.Museums.Any(e => e.Id == id);
        }
    }
} 