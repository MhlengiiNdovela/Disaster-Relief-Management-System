using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Service.Data;
using Service.Models;

namespace Web.Controllers;

[AllowAnonymous]
public class AccountController : Controller
{
    private readonly GiftDbContext _db;
    private readonly IPasswordHasher<User> _hasher;

    public AccountController(GiftDbContext db, IPasswordHasher<User> hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    [HttpGet]
    public IActionResult Login(string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password, string returnUrl = null)
    {
        var user = _db.Users.FirstOrDefault(u => u.Email == email);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View();
        }

        var res = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (res == PasswordVerificationResult.Success)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View();
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(string email, string password, string fullName)
    {
        if (_db.Users.Any(u => u.Email == email))
        {
            ModelState.AddModelError(string.Empty, "Email already in use.");
            return View();
        }

        var user = new User
        {
            Email = email,
            FirstName = fullName,
            Role = "User",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        user.PasswordHash = _hasher.HashPassword(user, password);
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        // Auto-login
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Role, user.Role ?? "User")
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied() => View();
}
