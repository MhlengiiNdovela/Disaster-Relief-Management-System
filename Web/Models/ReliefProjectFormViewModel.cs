using System.ComponentModel.DataAnnotations;

namespace Web.Models;

public class ReliefProjectFormViewModel
{
    [Required]
    [Display(Name = "Project title")]
    [StringLength(150)]
    public string Title { get; set; } = null!;

    [Required]
    [StringLength(255)]
    public string Location { get; set; } = null!;

    [Required]
    public string Description { get; set; } = null!;

    [Required]
    public string Status { get; set; } = "Active";
}
