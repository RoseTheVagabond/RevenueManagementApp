using System.ComponentModel.DataAnnotations;

namespace RevenueManagementApp.DTOs;

public class UpdateIndividualDto
{
    [Required]
    [StringLength(11, MinimumLength = 11)]
    public string Pesel { get; set; } = null!;

    [StringLength(50)]
    public string? FirstName { get; set; }

    [StringLength(50)]
    public string? LastName { get; set; }

    [StringLength(200)]
    public string? Address { get; set; }

    [StringLength(100)]
    [EmailAddress]
    public string? Email { get; set; }

    [StringLength(14)]
    public string? PhoneNumber { get; set; }
}