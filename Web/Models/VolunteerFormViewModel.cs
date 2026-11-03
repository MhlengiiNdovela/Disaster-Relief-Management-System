using System.ComponentModel.DataAnnotations;

namespace Web.Models;

public class VolunteerFormViewModel
{
    [Required]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [StringLength(250)]
    public string Skills { get; set; } = null!;

    [Required]
    public string Availability { get; set; } = null!;
}
