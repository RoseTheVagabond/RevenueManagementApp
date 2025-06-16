using RevenueManagementApp.Models;
using RevenueManagementApp.Repositories;

namespace RevenueManagementApp.Tests.Fakes;

public class FakeSalesRepository : ISalesRepository
{
    private readonly List<Discount> _discounts = new();
    private readonly List<Contract> _contracts = new();
    private readonly List<Software> _software = new();
    private int _nextDiscountId = 1;
    private int _nextContractId = 1;

    public Task<Discount> CreateDiscountAsync(Discount discount)
    {
        discount.Id = _nextDiscountId++;
        _discounts.Add(discount);
        return Task.FromResult(discount);
    }

    public Task<Discount?> GetDiscountByIdAsync(int id)
    {
        var discount = _discounts.FirstOrDefault(d => d.Id == id);
        return Task.FromResult(discount);
    }

    public Task<List<Discount>> GetActiveDiscountsAsync()
    {
        var now = DateTime.Now;
        var activeDiscounts = _discounts.Where(d => d.Start <= now && d.End >= now).ToList();
        return Task.FromResult(activeDiscounts);
    }

    public Task<int> CreateContractAsync(Contract contract)
    {
        contract.Id = _nextContractId++;
        _contracts.Add(contract);
        return Task.FromResult(contract.Id);
    }

    public Task<Contract?> GetContractByIdAsync(int id)
    {
        var contract = _contracts.FirstOrDefault(c => c.Id == id);
        return Task.FromResult(contract);
    }

    public Task<bool> UpdateContractAsync(Contract contract)
    {
        var existingContract = _contracts.FirstOrDefault(c => c.Id == contract.Id);
        if (existingContract == null)
            return Task.FromResult(false);

        var index = _contracts.IndexOf(existingContract);
        _contracts[index] = contract;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteContractAsync(int id)
    {
        var contract = _contracts.FirstOrDefault(c => c.Id == id);
        if (contract == null)
            return Task.FromResult(false);

        _contracts.Remove(contract);
        return Task.FromResult(true);
    }

    public Task<List<Contract>> GetContracts()
    {
        return Task.FromResult(_contracts.ToList());
    }

    public Task<List<Contract>> GetActiveContractsAsync()
    {
        var now = DateTime.UtcNow;
        var activeContracts = _contracts.Where(c => c.Start <= now && c.End >= now).ToList();
        return Task.FromResult(activeContracts);
    }

    public Task<Software?> GetSoftwareByIdAsync(int id)
    {
        var software = _software.FirstOrDefault(s => s.Id == id);
        return Task.FromResult(software);
    }

    public Task<List<Software>> GetAllSoftwareAsync()
    {
        return Task.FromResult(_software.ToList());
    }

    public Task<bool> HasActiveSubscriptionForSoftwareAsync(string? pesel, string? krs, int softwareId)
    {
        var now = DateTime.Now;
        
        var hasActiveSubscription = _contracts.Any(c => 
            c.SoftwareId == softwareId &&
            c.SoftwareDeadline > now &&
            c.IsSigned == true &&
            ((pesel != null && c.IndividualPesel == pesel) || (krs != null && c.CompanyKrs == krs)));
            
        return Task.FromResult(hasActiveSubscription);
    }

    public Task<decimal> GetCurrentRevenueAsync(int? softwareId = null)
    {
        var query = _contracts.Where(c => c.IsPaid == true && c.IsSigned == true);
        
        if (softwareId.HasValue)
        {
            query = query.Where(c => c.SoftwareId == softwareId.Value);
        }
        
        var revenue = query.Sum(c => c.Paid);
        return Task.FromResult(revenue);
    }

    public Task<decimal> GetPredictedRevenueAsync(int? softwareId = null)
    {
        var query = _contracts.AsQueryable();
        
        if (softwareId.HasValue)
        {
            query = query.Where(c => c.SoftwareId == softwareId.Value);
        }
        
        var currentRevenue = query
            .Where(c => c.IsPaid == true && c.IsSigned == true)
            .Sum(c => c.Paid);
        
        var unpaidRevenue = query
            .Where(c => c.IsPaid == false && c.End > DateTime.UtcNow)
            .Sum(c => c.ToPay);
        
        return Task.FromResult(currentRevenue + unpaidRevenue);
    }

    public Task<(int total, int paid, int unpaid)> GetContractStatsAsync(int? softwareId = null)
    {
        var query = _contracts.AsQueryable();
        
        if (softwareId.HasValue)
        {
            query = query.Where(c => c.SoftwareId == softwareId.Value);
        }
        
        var total = query.Count();
        var paid = query.Count(c => c.IsPaid == true);
        var unpaid = total - paid;
        
        return Task.FromResult((total, paid, unpaid));
    }
    
    public void AddTestSoftware(Software software)
    {
        _software.Add(software);
    }

    public void AddTestDiscount(Discount discount)
    {
        _discounts.Add(discount);
    }

    public void AddTestContract(Contract contract)
    {
        _contracts.Add(contract);
    }

    public void ClearData()
    {
        _discounts.Clear();
        _contracts.Clear();
        _software.Clear();
        _nextDiscountId = 1;
        _nextContractId = 1;
    }
}