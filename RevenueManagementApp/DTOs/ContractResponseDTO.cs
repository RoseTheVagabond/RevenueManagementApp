using System.ComponentModel.DataAnnotations;

namespace RevenueManagementApp.DTOs;

public class ContractResponseDTO
{
    public int Id { get; set; }
    
    [StringLength(11)]
    public string? IndividualPesel { get; set; }
    
    [StringLength(10)]
    public string? CompanyKrs { get; set; }

    public int SoftwareId { get; set; }
    public int? DiscountId { get; set; }

    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public DateTime SoftwareDeadline { get; set; }

    public bool IsSigned { get; set; }
    public bool IsPaid { get; set; }

    [Range(0, double.MaxValue)]
    public decimal ToPay { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Paid { get; set; }

    [Range(0, 3)]
    public int AdditionalSupportYears { get; set; }
    
    public decimal RemainingAmount => ToPay - Paid;
}