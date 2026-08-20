using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.Data;
using Service.Models;

namespace Web.Controllers;

public class ReliefRequestsController : Controller
{
    private readonly GiftDbContext _db;

    public ReliefRequestsController(GiftDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _db.ReliefRequests.Include(r => r.Project).ToListAsync();
        return View(list);
    }

    public IActionResult Create() => View(new ReliefRequest { RequestedAt = DateTime.UtcNow });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReliefRequest model)
    {
        if (!ModelState.IsValid) return View(model);
        _db.ReliefRequests.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _db.ReliefRequests.FindAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ReliefRequest model)
    {
        if (id != model.RequestId) return BadRequest();
        if (!ModelState.IsValid) return View(model);
        _db.Entry(model).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var item = await _db.ReliefRequests.Include(r => r.Project).FirstOrDefaultAsync(r => r.RequestId == id);
        if (item == null) return NotFound();
        return View(item);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.ReliefRequests.FindAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _db.ReliefRequests.FindAsync(id);
        if (item == null) return NotFound();
        _db.ReliefRequests.Remove(item);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
