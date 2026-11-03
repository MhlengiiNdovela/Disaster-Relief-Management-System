using System.ComponentModel.DataAnnotations;

namespace Web.Models;

public class DonationFormViewModel
{
    [Required]
    [Display(Name = "Donor name")]
    public string DonorName { get; set; } = null!;

    [EmailAddress]
    [Display(Name = "Email (optional)")]
    public string? Email { get; set; }

    [Required]
    [Range(1, 10000000)]
    public decimal Amount { get; set; }

    [Required]
    public string Currency { get; set; } = "ZAR";

    [Display(Name = "Recurring monthly donation")]
    public bool IsRecurring { get; set; }

    [Display(Name = "Relief project")]
    public int ReliefProjectId { get; set; }
}
