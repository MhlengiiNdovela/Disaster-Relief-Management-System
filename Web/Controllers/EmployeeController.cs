using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.Data;
using Service.Models;
using Web.Models;

namespace Web.Controllers;

[Authorize(Roles = "Employee,Admin")]
public class EmployeeController : Controller
{
    private readonly GiftDbContext _db;

    public EmployeeController(GiftDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.Volunteers = await _db.Volunteers.Include(v => v.User).OrderByDescending(v => v.CreatedAt).ToListAsync();
        ViewBag.Donations = await _db.Donations.Include(d => d.Project).OrderByDescending(d => d.DonationDate).Take(20).ToListAsync();
        ViewBag.Projects = await _db.ReliefProjects.OrderByDescending(p => p.CreatedAt).ToListAsync();
        return View(new ReliefProjectFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReliefProjectFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Volunteers = await _db.Volunteers.Include(v => v.User).OrderByDescending(v => v.CreatedAt).ToListAsync();
            ViewBag.Donations = await _db.Donations.Include(d => d.Project).OrderByDescending(d => d.DonationDate).Take(20).ToListAsync();
            ViewBag.Projects = await _db.ReliefProjects.OrderByDescending(p => p.CreatedAt).ToListAsync();
            return View("Index", model);
        }

        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            return Challenge();
        }

        _db.ReliefProjects.Add(new ReliefProject
        {
            ProjectName = model.Title,
            Location = model.Location,
            Description = model.Description,
            Status = model.Status,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
        TempData["ProjectPosted"] = model.Title;
        return RedirectToAction(nameof(Index));
    }
}
