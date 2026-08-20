using Microsoft.EntityFrameworkCore;
using Service.Data;
using Service.Interfaces;
using Service.Models;

namespace Service.Services;

public class ReliefService : IReliefService
{
    private readonly GiftDbContext _db;

    public ReliefService(GiftDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<ReliefRequest>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.ReliefRequests.AsNoTracking().ToListAsync(ct);
    }

    public async Task<ReliefRequest?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _db.ReliefRequests.FindAsync(new object[] { id }, ct);
    }

    public async Task<ReliefRequest> CreateAsync(ReliefRequest request, CancellationToken ct = default)
    {
        _db.ReliefRequests.Add(request);
        await _db.SaveChangesAsync(ct);
        return request;
    }
}
