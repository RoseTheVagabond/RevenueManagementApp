using RevenueManagementApp.DTOs;
using RevenueManagementApp.Models;

namespace RevenueManagementApp.Services;

public interface ISalesService
{
    Task<Discount> CreateDiscountAsync(DiscountDTO discountDto);
    Task<List<Discount>> GetActiveDiscountsAsync();
    Task<int> CreateContractAsync(ContractDTO contractDto);
    Task<bool> DeleteContractAsync(int contractId);
    Task<Contract> PayForContractAsync(ContractPaymentDTO paymentDto);
    Task<List<ContractResponseDTO>> GetContracts();
    Task<List<SoftwareResponseDTO>> GetAllSoftwareAsync();
    Task<RevenueResponseDTO> CalculateRevenueAsync(RevenueRequestDTO request);
    Task<decimal> GetExchangeRateAsync(string targetCurrency);
}