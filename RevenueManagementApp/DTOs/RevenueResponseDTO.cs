namespace RevenueManagementApp.DTOs;

public class RevenueResponseDTO
{
    public decimal CurrentRevenue { get; set; }
    public decimal PredictedRevenue { get; set; }
    public string Currency { get; set; } = "PLN";
    public decimal ExchangeRate { get; set; } = 1.0m;
    public int TotalContracts { get; set; }
    public int PaidContracts { get; set; }
    public int UnpaidContracts { get; set; }
    public string? SoftwareName { get; set; }
}