using Microsoft.AspNetCore.Mvc;
using RuinsandMuseums.Data;
using RuinsandMuseums.Models;
using RuinsandMuseums.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RuinsandMuseums.Controllers
{
    [AdminOnly]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Using ViewModel
            var viewModel = new AdminDashboardViewModel
            {
                TotalUsers = _context.Users.Count(),
                TotalRuins = _context.Ruins.Count(),
                TotalMuseums = _context.Museums.Count(),
                RecentUsers = _context.Users.OrderByDescending(u => u.CreatedAt).Take(5).ToList(),
                RecentRuins = _context.Ruins.OrderByDescending(r => r.Id).Take(5).ToList(),
                RecentMuseums = _context.Museums.OrderByDescending(m => m.Id).Take(5).ToList()
            };

            // Using ViewBag
            ViewBag.PageTitle = "Admin Dashboard";
            ViewBag.LastUpdated = DateTime.Now.ToString("g");

            // Using ViewData
            ViewData["WelcomeMessage"] = "Welcome to the Admin Panel";
            ViewData["TotalItems"] = viewModel.TotalRuins + viewModel.TotalMuseums;

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult AddRuin()
        {
            return View(new Ruin());
        }

        [HttpGet]
        public IActionResult AddMuseum()
        {
            return View(new Museum());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddRuin(Ruin ruin, IFormFile ImageFile)
        {
            if (!ModelState.IsValid)
            {
                return View(ruin);
            }

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(stream);
                }
                ruin.ImagePath = "/uploads/" + uniqueFileName;
            }

            _context.Ruins.Add(ruin);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Ruin added successfully!";
            return RedirectToAction("ManageRuinsAndMuseums");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddMuseum(Museum museum, IFormFile ImageFile)
        {
            if (!ModelState.IsValid)
            {
                return View(museum);
            }

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(stream);
                }
                museum.ImagePath = "/uploads/" + uniqueFileName;
            }

            _context.Museums.Add(museum);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Museum added successfully!";
            return RedirectToAction("ManageRuinsAndMuseums");
        }

        public IActionResult ManageUsers()
        {
            // Using ViewModel for user management
            var users = _context.Users.ToList();
            
            // Using ViewBag for additional data
            ViewBag.TotalUsers = users.Count;
            ViewBag.AdminCount = users.Count(u => u.IsAdmin);
            
            // Using ViewData for page information
            ViewData["PageTitle"] = "User Management";
            ViewData["LastUpdated"] = DateTime.Now.ToString("g");

            return View(users);
        }

        // CREATE USER
        [HttpGet]
        public IActionResult CreateUser()
        {
            ViewData["Title"] = "Add New User";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateUser(User user)
        {
            if (ModelState.IsValid)
            {
                user.CreatedAt = DateTime.UtcNow;
                _context.Users.Add(user);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "User created successfully!";
                return RedirectToAction("ManageUsers");
            }
            ViewData["Title"] = "Add New User";
            return View(user);
        }

        // EDIT USER
        [HttpGet]
        public IActionResult EditUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound();
            ViewData["Title"] = "Edit User";
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditUser(User user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = _context.Users.Find(user.Id);
                if (existingUser == null)
                    return NotFound();
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.Email = user.Email;
                existingUser.Address = user.Address;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.IsAdmin = user.IsAdmin;
                // Only update password if provided
                if (!string.IsNullOrEmpty(user.Password))
                    existingUser.Password = user.Password;
                _context.SaveChanges();
                TempData["SuccessMessage"] = "User updated successfully!";
                return RedirectToAction("ManageUsers");
            }
            ViewData["Title"] = "Edit User";
            return View(user);
        }

        // DELETE USER
        [HttpGet]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound();
            ViewData["Title"] = "Delete User";
            return View(user);
        }

        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteUserConfirmed(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound();
            _context.Users.Remove(user);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "User deleted successfully!";
            return RedirectToAction("ManageUsers");
        }

        public IActionResult ManageRuinsAndMuseums()
        {
            var viewModel = new RuinsAndMuseumsViewModel
            {
                Ruins = _context.Ruins.ToList(),
                Museums = _context.Museums.ToList()
            };
            return View(viewModel);
        }

        // Edit Ruin
        [HttpGet]
        public IActionResult EditRuin(int id)
        {
            var ruin = _context.Ruins.Find(id);
            if (ruin == null)
                return NotFound();

            var viewModel = new RuinsAndMuseumsViewModel
            {
                NewRuin = ruin
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditRuin(RuinsAndMuseumsViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var existingRuin = _context.Ruins.Find(viewModel.NewRuin.Id);
                if (existingRuin == null)
                    return NotFound();

                existingRuin.Name = viewModel.NewRuin.Name;
                existingRuin.Location = viewModel.NewRuin.Location;
                existingRuin.Description = viewModel.NewRuin.Description;

                if (viewModel.NewRuin.ImageFile != null && viewModel.NewRuin.ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);
                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(viewModel.NewRuin.ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        viewModel.NewRuin.ImageFile.CopyTo(stream);
                    }
                    existingRuin.ImagePath = "/uploads/" + uniqueFileName;
                }

                _context.SaveChanges();
                TempData["SuccessMessage"] = "Ruin updated successfully!";
                return RedirectToAction("ManageRuinsAndMuseums");
            }
            return View(viewModel);
        }

        // Delete Ruin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteRuin(int id)
        {
            var ruin = _context.Ruins.Find(id);
            if (ruin == null)
                return NotFound();

            _context.Ruins.Remove(ruin);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Ruin deleted successfully!";
            return RedirectToAction("ManageRuinsAndMuseums");
        }

        // Edit Museum
        [HttpGet]
        public IActionResult EditMuseum(int id)
        {
            var museum = _context.Museums.Find(id);
            if (museum == null)
                return NotFound();

            var viewModel = new RuinsAndMuseumsViewModel
            {
                NewMuseum = museum
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditMuseum(RuinsAndMuseumsViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var existingMuseum = _context.Museums.Find(viewModel.NewMuseum.Id);
                if (existingMuseum == null)
                    return NotFound();

                existingMuseum.Name = viewModel.NewMuseum.Name;
                existingMuseum.Location = viewModel.NewMuseum.Location;
                existingMuseum.Description = viewModel.NewMuseum.Description;

                if (viewModel.NewMuseum.ImageFile != null && viewModel.NewMuseum.ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);
                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(viewModel.NewMuseum.ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        viewModel.NewMuseum.ImageFile.CopyTo(stream);
                    }
                    existingMuseum.ImagePath = "/uploads/" + uniqueFileName;
                }

                _context.SaveChanges();
                TempData["SuccessMessage"] = "Museum updated successfully!";
                return RedirectToAction("ManageRuinsAndMuseums");
            }
            return View(viewModel);
        }

        // Delete Museum
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteMuseum(int id)
        {
            var museum = _context.Museums.Find(id);
            if (museum == null)
                return NotFound();

            _context.Museums.Remove(museum);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Museum deleted successfully!";
            return RedirectToAction("ManageRuinsAndMuseums");
        }
    }
} 