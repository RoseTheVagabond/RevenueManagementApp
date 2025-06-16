using System.ComponentModel.DataAnnotations;

namespace RevenueManagementApp.DTOs;

public class DiscountDTO
{
    [Required]
    [Range(1, 100, ErrorMessage = "Percentage must be between 1 and 100")]
    public int Percentage { get; set; }

    [Required]
    public DateTime Start { get; set; }

    [Required]
    public DateTime End { get; set; }
}