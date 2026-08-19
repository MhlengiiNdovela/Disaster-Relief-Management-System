using System;
using System.Collections.Generic;

namespace Disaster_Relief_Management_System.Services.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Donation> Donations { get; set; } = new List<Donation>();

    public virtual ICollection<ReliefProject> ReliefProjects { get; set; } = new List<ReliefProject>();

    public virtual ICollection<ReliefRequest> ReliefRequests { get; set; } = new List<ReliefRequest>();

    public virtual ICollection<ResourceAllocation> ResourceAllocations { get; set; } = new List<ResourceAllocation>();

    public virtual Volunteer? Volunteer { get; set; }
}
