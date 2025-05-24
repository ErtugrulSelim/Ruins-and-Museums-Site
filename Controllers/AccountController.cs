using System.Diagnostics;
using RuinsandMuseums.Models;
using Microsoft.AspNetCore.Mvc;
using RuinsandMuseums.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    [Route("/Login")]
    public IActionResult Login()
    {
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Profile");
        }
        return View();
    }

    // POST: /Account/Login
    [HttpPost]
    [Route("/Login")]
    public async Task<IActionResult> Login(string email, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

        if (user == null)
        {
            ModelState.AddModelError("", "Invalid email or password");
            return View();
        }

        // Create claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User"),
            new Claim("FullName", user.FullName ?? "")
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        return RedirectToAction("Profile");
    }

    [Route("/Register")]
    public IActionResult Register()
    {
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Profile");
        }
        return View();
    }

    // POST: /Account/Register
    [HttpPost]
    [Route("/Register")]
    public async Task<IActionResult> Register(User user)
    {
        if (ModelState.IsValid)
        {
            // Check if email already exists
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Email already exists");
                return View(user);
            }

            // Set default values
            user.CreatedAt = DateTime.UtcNow;
            user.IsAdmin = false; // New users are not admins by default

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Automatically log in the user after registration
            return await Login(user.Email, user.Password);
        }
        return View(user);
    }

    public async Task<IActionResult> Logout()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login");
        }
        return View();
    }

    // POST: /Account/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout(string returnUrl)
    {
        // Sign out
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (string.IsNullOrEmpty(returnUrl))
        {
            return RedirectToAction("Login");
        }
        else
        {
            return Redirect(returnUrl);
        }
    }

    [Route("/Profile")]
    public IActionResult Profile()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login");
        }

        var email = User.Identity.Name;
        var user = _context.Users.FirstOrDefault(u => u.Email == email);
        if (user == null)
        {
            return RedirectToAction("Logout");
        }
        return View(user);
    }

    [Route("/AccessDenied")]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [HttpGet]
    public IActionResult EditProfile()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login");
        }

        var user = _context.Users.Find(userId);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditProfile(User user)
    {
        if (ModelState.IsValid)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null || userId != user.Id)
            {
                return RedirectToAction("Login");
            }

            var existingUser = _context.Users.Find(userId);
            if (existingUser == null)
            {
                return NotFound();
            }

            // Update user properties
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.Address = user.Address;

            // Only update password if a new one is provided
            if (!string.IsNullOrEmpty(user.Password))
            {
                existingUser.Password = user.Password;
            }

            _context.SaveChanges();
            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("Profile");
        }

        return View(user);
    }
}