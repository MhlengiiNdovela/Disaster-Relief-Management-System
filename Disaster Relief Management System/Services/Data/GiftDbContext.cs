using Microsoft.EntityFrameworkCore;
using Disaster_Relief_Management_System.Services.Models;

namespace Disaster_Relief_Management_System.Services.Data;

public class GiftDbContext : DbContext
{
    public GiftDbContext(DbContextOptions<GiftDbContext> options) : base(options)
    {
    }

    public DbSet<ReliefRequest> ReliefRequests => Set<ReliefRequest>();
}
