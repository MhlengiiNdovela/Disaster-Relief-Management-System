using System;
using System.Collections.Generic;

namespace Service.Models;

public partial class Volunteer
{
    public int VolunteerId { get; set; }

    public int UserId { get; set; }

    public string? Address { get; set; }

    public string? Availability { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsAvailable { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Skills { get; set; }

    public virtual User User { get; set; } = null!;
}
