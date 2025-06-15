using RevenueManagementApp.DTOs;
using RevenueManagementApp.Models;

namespace RevenueManagementApp.Services;

public interface IClientService
{
    Task<List<Individual>> GetAllIndividualsAsync();
    Task<List<Company>> GetAllCompaniesAsync();
    Task<object> GetAllClientsAsync();
    Task<Individual> AddIndividualAsync(CreateIndividualDto createIndividualDto);
    Task<Company> AddCompanyAsync(CreateCompanyDto createCompanyDto);
    Task<Individual?> UpdateIndividualAsync(UpdateIndividualDto updateIndividualDto);
    Task<Company?> UpdateCompanyAsync(UpdateCompanyDto updateCompanyDto);
    Task<bool> DeleteIndividualAsync(string pesel);
}