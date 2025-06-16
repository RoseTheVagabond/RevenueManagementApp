using Microsoft.EntityFrameworkCore;
using RevenueManagementApp.Models;

namespace RevenueManagementApp.Repositories;

public class SalesRepository : ISalesRepository
{
    private readonly MasterContext _context;

    public SalesRepository(MasterContext context)
    {
        _context = context;
    }
    
    public async Task<Discount> CreateDiscountAsync(Discount discount)
    {
        var maxId = await _context.Discounts.MaxAsync(d => (int?)d.Id) ?? 0;
        discount.Id = maxId + 1;
        
        _context.Discounts.Add(discount);
        await _context.SaveChangesAsync();
        return discount;
    }

    public async Task<Discount?> GetDiscountByIdAsync(int id)
    {
        return await _context.Discounts.FindAsync(id);
    }

    public async Task<List<Discount>> GetActiveDiscountsAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.Discounts
            .Where(d => d.Start <= now && d.End >= now)
            .ToListAsync();
    }
    
    public async Task<int> CreateContractAsync(Contract contract)
    {
        var maxId = await _context.Contracts.MaxAsync(c => (int?)c.Id) ?? 0;
        contract.Id = maxId + 1;
        
        _context.Contracts.Add(contract);
        await _context.SaveChangesAsync();
        return contract.Id;
    }

    public async Task<Contract?> GetContractByIdAsync(int id)
    {
        return await _context.Contracts
            .Include(c => c.Individual)
            .Include(c => c.Company)
            .Include(c => c.Software)
            .Include(c => c.Discount)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<bool> UpdateContractAsync(Contract contract)
    {
        _context.Contracts.Update(contract);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> DeleteContractAsync(int id)
    {
        var contract = await GetContractByIdAsync(id);
        if (contract == null)
            return false;

        _context.Contracts.Remove(contract);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<List<Contract>> GetContracts()
    {
        return await _context.Contracts.ToListAsync();
    }

    public async Task<List<Contract>> GetActiveContractsAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.Contracts
            .Include(c => c.Individual)
            .Include(c => c.Company)
            .Include(c => c.Software)
            .Include(c => c.Discount)
            .Where(c => c.Start <= now && c.End >= now)
            .ToListAsync();
    }
    
    public async Task<Software?> GetSoftwareByIdAsync(int id)
    {
        return await _context.Softwares
            .Include(s => s.Cathegory)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Software>> GetAllSoftwareAsync()
    {
        return await _context.Softwares
            .Include(s => s.Cathegory)
            .ToListAsync();
    }
    
    public async Task<bool> HasActiveSubscriptionForSoftwareAsync(string? pesel, string? krs, int softwareId)
    {
        var now = DateTime.Now;
        
        var query = _context.Contracts
            .Where(c => c.SoftwareId == softwareId &&
                        c.SoftwareDeadline > now &&
                        c.IsSigned == true);
        
        if (!string.IsNullOrEmpty(pesel))
        {
            query = query.Where(c => c.IndividualPesel == pesel);
        }
        else if (!string.IsNullOrEmpty(krs))
        {
            query = query.Where(c => c.CompanyKrs == krs);
        }
        else
        {
            return false;
        }
    
        return await query.AnyAsync();
    }
    
    public async Task<decimal> GetCurrentRevenueAsync(int? softwareId = null)
    {
        var query = _context.Contracts
            .Where(c => c.IsPaid == true && c.IsSigned == true);
    
        if (softwareId.HasValue)
        {
            query = query.Where(c => c.SoftwareId == softwareId.Value);
        }
    
        return await query.SumAsync(c => c.Paid);
    }

    public async Task<decimal> GetPredictedRevenueAsync(int? softwareId = null)
    {
        var query = _context.Contracts.AsQueryable();
    
        if (softwareId.HasValue)
        {
            query = query.Where(c => c.SoftwareId == softwareId.Value);
        }
        
        var currentRevenue = await query
            .Where(c => c.IsPaid == true && c.IsSigned == true)
            .SumAsync(c => c.Paid);
        
        var unpaidRevenue = await query
            .Where(c => c.IsPaid == false && c.End > DateTime.UtcNow) // Still in payment period
            .SumAsync(c => c.ToPay);
    
        return currentRevenue + unpaidRevenue;
    }

    public async Task<(int total, int paid, int unpaid)> GetContractStatsAsync(int? softwareId = null)
    {
        var query = _context.Contracts.AsQueryable();
    
        if (softwareId.HasValue)
        {
            query = query.Where(c => c.SoftwareId == softwareId.Value);
        }
    
        var total = await query.CountAsync();
        var paid = await query.CountAsync(c => c.IsPaid == true);
        var unpaid = total - paid;
    
        return (total, paid, unpaid);
    }
}