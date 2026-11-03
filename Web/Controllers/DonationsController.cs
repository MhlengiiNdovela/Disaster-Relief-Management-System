using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Service.Data;
using Service.Models;
using GiftOfTheGivers.Helpers;
using Web.Models;
using Web.Services;

namespace Web.Controllers;

public class DonationsController : Controller
{
    private readonly GiftDbContext _db;

    public DonationsController(GiftDbContext db)
    {
        _db = db;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        await LoadProjectsAsync();
        return View(new DonationFormViewModel());
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(DonationFormViewModel model)
    {
        if (!DonationRules.IsValid(model.DonorName, model.Amount, model.Currency))
        {
            ModelState.AddModelError(string.Empty, "Enter a donor name, a positive amount, and ZAR, USD, or EUR.");
        }

        if (!ModelState.IsValid)
        {
            await LoadProjectsAsync();
            return View(model);
        }

        var project = await _db.ReliefProjects.FirstOrDefaultAsync(p => p.ProjectId == model.ReliefProjectId)
            ?? await _db.ReliefProjects.OrderBy(p => p.ProjectId).FirstAsync();

        int? donorUserId = null;
        if (User.Identity?.IsAuthenticated == true
            && int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            donorUserId = userId;
        }

        var donation = new Donation
        {
            ProjectId = project.ProjectId,
            DonorUserId = donorUserId,
            DonationType = model.IsRecurring ? "Recurring" : "One-time",
            Amount = model.Amount,
            Currency = model.Currency,
            Description = model.DonorName,
            DonationDate = DateTime.UtcNow,
            Status = "Received"
        };
        _db.Donations.Add(donation);
        await _db.SaveChangesAsync();

        TempData["DonorEmail"] = model.Email;
        return RedirectToAction(nameof(Receipt), new { id = donation.DonationId });
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Receipt(int id)
    {
        var donation = await _db.Donations.Include(d => d.Project).FirstOrDefaultAsync(d => d.DonationId == id);
        if (donation is null)
        {
            return NotFound();
        }

        return View(donation);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Certificate(int id)
    {
        var donation = await _db.Donations.Include(d => d.Project).FirstOrDefaultAsync(d => d.DonationId == id);
        if (donation is null)
        {
            return NotFound();
        }

        var donatedOn = DateOnly.FromDateTime(donation.DonationDate);
        var pdf = TaxCertificatePdf.Create(new DonationCertificateRequest(
            donation.Description ?? "Donor",
            donation.Amount ?? 0,
            donation.Currency ?? "ZAR",
            donatedOn,
            donation.Project.ProjectName,
            CertificateFormatter.FormatNumber(donation.DonationId, donatedOn)));

        return File(pdf, "application/pdf", "donation-tax-certificate.pdf");
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Mine()
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            return Challenge();
        }

        var donations = await _db.Donations
            .Include(d => d.Project)
            .Where(d => d.DonorUserId == userId)
            .OrderByDescending(d => d.DonationDate)
            .ToListAsync();
        return View(donations);
    }

    private async Task LoadProjectsAsync()
    {
        var projects = await _db.ReliefProjects.OrderBy(p => p.ProjectName).ToListAsync();
        ViewBag.Projects = new SelectList(projects, "ProjectId", "ProjectName");
    }
}
