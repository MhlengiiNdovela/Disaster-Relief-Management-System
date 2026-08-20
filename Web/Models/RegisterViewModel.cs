using System.ComponentModel.DataAnnotations;

namespace Web.Models;

public class RegisterViewModel
{
    [Required]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
}
