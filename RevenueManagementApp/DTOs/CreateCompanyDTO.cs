using System.ComponentModel.DataAnnotations;

namespace RevenueManagementApp.DTOs;

public class CreateCompanyDto
{
    [Required]
    [StringLength(10, MinimumLength = 10)]
    public string Krs { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(200)]
    public string Address { get; set; } = null!;

    [Required]
    [StringLength(100)]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [StringLength(14)]
    public string PhoneNumber { get; set; } = null!;
}