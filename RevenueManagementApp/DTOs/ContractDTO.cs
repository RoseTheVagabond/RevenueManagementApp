using System.ComponentModel.DataAnnotations;

namespace RevenueManagementApp.DTOs;

public class ContractDTO
{
    [StringLength(11, MinimumLength = 11)]
    public string? IndividualPesel { get; set; }
    
    [StringLength(10, MinimumLength = 10)]
    public string? CompanyKrs { get; set; }

    [Required]
    public int SoftwareId { get; set; }

    [Required]
    public DateTime Start { get; set; }

    [Required]
    public DateTime End { get; set; }

    // Optional: number of years of additional support (0-3)
    [Range(0, 3)]
    public int AdditionalSupportYears { get; set; } = 0;
}