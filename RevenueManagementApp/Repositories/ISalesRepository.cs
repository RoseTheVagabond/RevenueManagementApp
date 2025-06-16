using RevenueManagementApp.Models;

namespace RevenueManagementApp.Repositories;

public interface ISalesRepository
{
    Task<Discount> CreateDiscountAsync(Discount discount);
    Task<Discount?> GetDiscountByIdAsync(int id);
    Task<List<Discount>> GetActiveDiscountsAsync();
    Task<int> CreateContractAsync(Contract contract);
    Task<Contract?> GetContractByIdAsync(int id);
    Task<bool> UpdateContractAsync(Contract contract);
    Task<bool> DeleteContractAsync(int id);
    Task<List<Contract>> GetContracts();
    Task<List<Contract>> GetActiveContractsAsync();
    Task<Software?> GetSoftwareByIdAsync(int id);
    Task<List<Software>> GetAllSoftwareAsync();
    Task<bool> HasActiveSubscriptionForSoftwareAsync(string? pesel, string? krs, int softwareId);
}