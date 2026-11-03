using System;
using System.Collections.Generic;

namespace Service.Models;

public partial class Donation
{
    public int DonationId { get; set; }

    public int ProjectId { get; set; }

    public int? DonorUserId { get; set; }

    public string DonationType { get; set; } = null!;

    public decimal? Amount { get; set; }

    public string? Currency { get; set; }

    public string? Description { get; set; }

    public DateTime DonationDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual User? DonorUser { get; set; }

    public virtual ReliefProject Project { get; set; } = null!;
}
