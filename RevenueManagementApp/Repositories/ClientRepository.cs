using Microsoft.EntityFrameworkCore;
using RevenueManagementApp.Models;

namespace RevenueManagementApp.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly MasterContext _context;

    public ClientRepository(MasterContext context)
    {
        _context = context;
    }

    public async Task<Individual?> GetIndividualByPeselAsync(string pesel)
    {
        return await _context.Individuals
            .FirstOrDefaultAsync(i => i.Pesel == pesel && i.DeletedAt == null);
    }

    public async Task<Company?> GetCompanyByKrsAsync(string krs)
    {
        return await _context.Companies
            .FirstOrDefaultAsync(c => c.Krs == krs);
    }

    public async Task<Individual> AddIndividualAsync(Individual individual)
    {
        _context.Individuals.Add(individual);
        await _context.SaveChangesAsync();
        return individual;
    }

    public async Task<Company> AddCompanyAsync(Company company)
    {
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();
        return company;
    }

    public async Task<Individual?> UpdateIndividualAsync(Individual individual)
    {
        var existingIndividual = await GetIndividualByPeselAsync(individual.Pesel);
        if (existingIndividual == null)
            return null;
        
        if (!string.IsNullOrEmpty(individual.FirstName))
            existingIndividual.FirstName = individual.FirstName;
        
        if (!string.IsNullOrEmpty(individual.LastName))
            existingIndividual.LastName = individual.LastName;
        
        if (!string.IsNullOrEmpty(individual.Address))
            existingIndividual.Address = individual.Address;
        
        if (!string.IsNullOrEmpty(individual.Email))
            existingIndividual.Email = individual.Email;
        
        if (!string.IsNullOrEmpty(individual.PhoneNumber))
            existingIndividual.PhoneNumber = individual.PhoneNumber;

        await _context.SaveChangesAsync();
        return existingIndividual;
    }

    public async Task<Company?> UpdateCompanyAsync(Company company)
    {
        var existingCompany = await GetCompanyByKrsAsync(company.Krs);
        if (existingCompany == null)
            return null;
        
        if (!string.IsNullOrEmpty(company.Name))
            existingCompany.Name = company.Name;
        
        if (!string.IsNullOrEmpty(company.Address))
            existingCompany.Address = company.Address;
        
        if (!string.IsNullOrEmpty(company.Email))
            existingCompany.Email = company.Email;
        
        if (!string.IsNullOrEmpty(company.PhoneNumber))
            existingCompany.PhoneNumber = company.PhoneNumber;

        await _context.SaveChangesAsync();
        return existingCompany;
    }

    public async Task<bool> SoftDeleteIndividualAsync(string pesel)
    {
        var individual = await GetIndividualByPeselAsync(pesel);
        if (individual == null)
            return false;

        individual.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IndividualExistsAsync(string pesel)
    {
        return await _context.Individuals
            .AnyAsync(i => i.Pesel == pesel && i.DeletedAt == null);
    }

    public async Task<bool> CompanyExistsAsync(string krs)
    {
        return await _context.Companies
            .AnyAsync(c => c.Krs == krs);
    }
}