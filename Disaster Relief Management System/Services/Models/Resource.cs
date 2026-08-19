using System;
using System.Collections.Generic;

namespace Disaster_Relief_Management_System.Services.Models;

public partial class Resource
{
    public int ResourceId { get; set; }

    public string ResourceName { get; set; } = null!;

    public string ResourceType { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Quantity { get; set; }

    public string? Unit { get; set; }

    public string? Location { get; set; }

    public bool IsAvailable { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<ResourceAllocation> ResourceAllocations { get; set; } = new List<ResourceAllocation>();
}
