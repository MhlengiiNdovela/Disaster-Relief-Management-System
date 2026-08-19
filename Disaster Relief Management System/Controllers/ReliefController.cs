using Microsoft.AspNetCore.Mvc;
using Disaster_Relief_Management_System.Services.Interfaces;
using Disaster_Relief_Management_System.Services.Models;

namespace Disaster_Relief_Management_System.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReliefController : ControllerBase
{
    private readonly IReliefService _service;

    public ReliefController(IReliefService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _service.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ReliefRequest request)
    {
        var created = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }
}
