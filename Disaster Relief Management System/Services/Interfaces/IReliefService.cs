using Disaster_Relief_Management_System.Services.Models;

namespace Disaster_Relief_Management_System.Services.Interfaces;

public interface IReliefService
{
    Task<IEnumerable<ReliefRequest>> GetAllAsync(CancellationToken ct = default);
    Task<ReliefRequest?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ReliefRequest> CreateAsync(ReliefRequest request, CancellationToken ct = default);
}
