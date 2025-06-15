using System.ComponentModel.DataAnnotations;

namespace RevenueManagementApp.Models;

public partial class Individual
{
    [Key]
    [StringLength(11, MinimumLength = 11)]
    public string Pesel { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = null!;

    [Required]
    [StringLength(200)]
    public string Address { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string Email { get; set; } = null!;

    [Required]
    [StringLength(14)]
    public string PhoneNumber { get; set; } = null!;
    
    public DateTime? DeletedAt { get; set; }
    
    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}