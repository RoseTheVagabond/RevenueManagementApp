using RevenueManagementApp.Models;

namespace RevenueManagementApp.Repositories;

public interface IClientRepository
{
    Task<Individual?> GetIndividualByPeselAsync(string pesel);
    Task<Company?> GetCompanyByKrsAsync(string krs);
    Task<List<Individual>> GetAllIndividualsAsync();
    Task<List<Company>> GetAllCompaniesAsync();
    Task<Individual> AddIndividualAsync(Individual individual);
    Task<Company> AddCompanyAsync(Company company);
    Task<Individual?> UpdateIndividualAsync(Individual individual);
    Task<Company?> UpdateCompanyAsync(Company company);
    Task<bool> SoftDeleteIndividualAsync(string pesel);
    Task<bool> IndividualExistsAsync(string pesel);
    Task<bool> CompanyExistsAsync(string krs);
}