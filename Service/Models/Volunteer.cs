using System;
using System.Collections.Generic;

namespace Service.Models;

public partial class Volunteer
{
    public int VolunteerId { get; set; }

    public int UserId { get; set; }

    public string? Address { get; set; }

    public string? Availability { get; set; }

    public virtual User User { get; set; } = null!;
}
