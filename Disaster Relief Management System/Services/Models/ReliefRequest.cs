namespace Disaster_Relief_Management_System.Services.Models;

public class ReliefRequest
{
    public int Id { get; set; }
    public string? RequesterName { get; set; }
    public string? Description { get; set; }
    public DateTime RequestedAt { get; set; }
}
