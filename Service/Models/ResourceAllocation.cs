using System;
using System.Collections.Generic;

namespace Service.Models;

public partial class ResourceAllocation
{
    public int AllocationId { get; set; }

    public int ProjectId { get; set; }

    public int ResourceId { get; set; }

    public int AllocatedByUserId { get; set; }

    public decimal Quantity { get; set; }

    public DateTime AllocationDate { get; set; }

    public string? Notes { get; set; }

    public virtual User AllocatedByUser { get; set; } = null!;

    public virtual ReliefProject Project { get; set; } = null!;

    public virtual Resource Resource { get; set; } = null!;
}
