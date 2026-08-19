using System;
using System.Collections.Generic;

namespace Disaster_Relief_Management_System.Services.Models;

public partial class ReliefRequest
{
    public int RequestId { get; set; }

    public int ProjectId { get; set; }

    public int? RequestedByUserId { get; set; }

    public string RequestType { get; set; } = null!;

    public string? Description { get; set; }

    public int? Quantity { get; set; }

    public string Priority { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime RequestedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public virtual ReliefProject Project { get; set; } = null!;

    public virtual User? RequestedByUser { get; set; }
}
