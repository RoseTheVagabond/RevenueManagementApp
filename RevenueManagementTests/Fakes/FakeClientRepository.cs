// Fakes/FakeClientRepository.cs
using RevenueManagementApp.Models;
using RevenueManagementApp.Repositories;

namespace RevenueManagementApp.Tests.Fakes;

public class FakeClientRepository : IClientRepository
{
    private readonly List<Individual> _individuals = new();
    private readonly List<Company> _companies = new();

    public Task<Individual?> GetIndividualByPeselAsync(string pesel)
    {
        var individual = _individuals.FirstOrDefault(i => i.Pesel == pesel && i.DeletedAt == null);
        return Task.FromResult(individual);
    }

    public Task<Company?> GetCompanyByKrsAsync(string krs)
    {
        var company = _companies.FirstOrDefault(c => c.Krs == krs);
        return Task.FromResult(company);
    }

    public Task<List<Individual>> GetAllIndividualsAsync()
    {
        return Task.FromResult(_individuals.Where(i => i.DeletedAt == null).ToList());
    }

    public Task<List<Company>> GetAllCompaniesAsync()
    {
        return Task.FromResult(_companies.ToList());
    }

    public Task<Individual> AddIndividualAsync(Individual individual)
    {
        _individuals.Add(individual);
        return Task.FromResult(individual);
    }

    public Task<Company> AddCompanyAsync(Company company)
    {
        _companies.Add(company);
        return Task.FromResult(company);
    }

    public Task<Individual?> UpdateIndividualAsync(Individual individual)
    {
        var existingIndividual = _individuals.FirstOrDefault(i => i.Pesel == individual.Pesel && i.DeletedAt == null);
        if (existingIndividual == null)
            return Task.FromResult<Individual?>(null);

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

        return Task.FromResult<Individual?>(existingIndividual);
    }

    public Task<Company?> UpdateCompanyAsync(Company company)
    {
        var existingCompany = _companies.FirstOrDefault(c => c.Krs == company.Krs);
        if (existingCompany == null)
            return Task.FromResult<Company?>(null);

        if (!string.IsNullOrEmpty(company.Name))
            existingCompany.Name = company.Name;
        
        if (!string.IsNullOrEmpty(company.Address))
            existingCompany.Address = company.Address;
        
        if (!string.IsNullOrEmpty(company.Email))
            existingCompany.Email = company.Email;
        
        if (!string.IsNullOrEmpty(company.PhoneNumber))
            existingCompany.PhoneNumber = company.PhoneNumber;

        return Task.FromResult<Company?>(existingCompany);
    }

    public Task<bool> SoftDeleteIndividualAsync(string pesel)
    {
        var individual = _individuals.FirstOrDefault(i => i.Pesel == pesel && i.DeletedAt == null);
        if (individual == null)
            return Task.FromResult(false);

        individual.DeletedAt = DateTime.Now;
        return Task.FromResult(true);
    }

    public Task<bool> IndividualExistsAsync(string pesel)
    {
        var exists = _individuals.Any(i => i.Pesel == pesel);
        return Task.FromResult(exists);
    }

    public Task<bool> CompanyExistsAsync(string krs)
    {
        var exists = _companies.Any(c => c.Krs == krs);
        return Task.FromResult(exists);
    }

    // Helper methods for testing
    public void AddTestIndividual(Individual individual)
    {
        _individuals.Add(individual);
    }

    public void AddTestCompany(Company company)
    {
        _companies.Add(company);
    }

    public void ClearData()
    {
        _individuals.Clear();
        _companies.Clear();
    }
}
