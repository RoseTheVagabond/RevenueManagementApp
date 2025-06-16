using System.ComponentModel.DataAnnotations;

namespace RevenueManagementApp.DTOs;

public class ContractPaymentDTO
{
    [Required]
    public int ContractId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Payment amount must be greater than 0")]
    public decimal PaymentAmount { get; set; }
}