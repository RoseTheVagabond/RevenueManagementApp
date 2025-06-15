using System.ComponentModel.DataAnnotations;

namespace RevenueManagementApp.DTOs;

public class UpdateCompanyDto
{
    [Required]
    [StringLength(10, MinimumLength = 10)]
    public string Krs { get; set; } = null!;

    [StringLength(100)]
    public string? Name { get; set; }

    [StringLength(200)]
    public string? Address { get; set; }

    [StringLength(100)]
    [EmailAddress]
    public string? Email { get; set; }

    [StringLength(14)]
    public string? PhoneNumber { get; set; }
}