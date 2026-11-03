using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.Data;
using Service.Models;
using Web.Models;

namespace Web.Controllers;

[AllowAnonymous]
public class VolunteersController : Controller
{
    private readonly GiftDbContext _db;
    private readonly IPasswordHasher<User> _hasher;

    public VolunteersController(GiftDbContext db, IPasswordHasher<User> hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    [HttpGet]
    public IActionResult Register() => View(new VolunteerFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(VolunteerFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
        if (user is null)
        {
            var names = model.FullName.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            user = new User
            {
                Email = model.Email,
                FirstName = names.Length > 0 ? names[0] : model.FullName,
                LastName = names.Length > 1 ? names[1] : string.Empty,
                Role = "Volunteer",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            user.PasswordHash = _hasher.HashPassword(user, Guid.NewGuid().ToString("N"));
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        var existing = await _db.Volunteers.FirstOrDefaultAsync(v => v.UserId == user.UserId);
        if (existing is null)
        {
            _db.Volunteers.Add(new Volunteer
            {
                UserId = user.UserId,
                Skills = model.Skills,
                Availability = model.Availability,
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();
        }
        else
        {
            existing.Skills = model.Skills;
            existing.Availability = model.Availability;
            existing.IsAvailable = true;
            await _db.SaveChangesAsync();
        }

        TempData["VolunteerName"] = model.FullName;
        return RedirectToAction(nameof(Thanks));
    }

    [HttpGet]
    public IActionResult Thanks() => View();
}
