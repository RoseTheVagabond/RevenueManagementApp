using RevenueManagementApp.DTOs;
using RevenueManagementApp.Models;
using RevenueManagementApp.Repositories;

namespace RevenueManagementApp.Services;

public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;

    public ClientService(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<List<Individual>> GetAllIndividualsAsync()
    {
        return await _clientRepository.GetAllIndividualsAsync();
    }

    public async Task<List<Company>> GetAllCompaniesAsync()
    {
        return await _clientRepository.GetAllCompaniesAsync();
    }

    public async Task<object> GetAllClientsAsync()
    {
        var individuals = await _clientRepository.GetAllIndividualsAsync();
        var companies = await _clientRepository.GetAllCompaniesAsync();

        return new
        {
            Individuals = individuals,
            Companies = companies,
            TotalClients = individuals.Count + companies.Count
        };
    }

    public async Task<Individual> AddIndividualAsync(CreateIndividualDto createIndividualDto)
    {
        // Check if individual already exists
        if (await _clientRepository.IndividualExistsAsync(createIndividualDto.Pesel))
        {
            throw new InvalidOperationException($"Individual with PESEL {createIndividualDto.Pesel} already exists.");
        }

        var individual = new Individual
        {
            Pesel = createIndividualDto.Pesel,
            FirstName = createIndividualDto.FirstName,
            LastName = createIndividualDto.LastName,
            Address = createIndividualDto.Address,
            Email = createIndividualDto.Email,
            PhoneNumber = createIndividualDto.PhoneNumber
        };

        return await _clientRepository.AddIndividualAsync(individual);
    }

    public async Task<Company> AddCompanyAsync(CreateCompanyDto createCompanyDto)
    {
        // Check if company already exists
        if (await _clientRepository.CompanyExistsAsync(createCompanyDto.Krs))
        {
            throw new InvalidOperationException($"Company with KRS {createCompanyDto.Krs} already exists.");
        }

        var company = new Company
        {
            Krs = createCompanyDto.Krs,
            Name = createCompanyDto.Name,
            Address = createCompanyDto.Address,
            Email = createCompanyDto.Email,
            PhoneNumber = createCompanyDto.PhoneNumber
        };

        return await _clientRepository.AddCompanyAsync(company);
    }

    public async Task<Individual?> UpdateIndividualAsync(UpdateIndividualDto updateIndividualDto)
    {
        // Check if individual exists
        var existingIndividual = await _clientRepository.GetIndividualByPeselAsync(updateIndividualDto.Pesel);
        if (existingIndividual == null)
        {
            throw new KeyNotFoundException($"Individual with PESEL {updateIndividualDto.Pesel} not found.");
        }

        var individualToUpdate = new Individual
        {
            Pesel = updateIndividualDto.Pesel,
            FirstName = updateIndividualDto.FirstName,
            LastName = updateIndividualDto.LastName,
            Address = updateIndividualDto.Address,
            Email = updateIndividualDto.Email,
            PhoneNumber = updateIndividualDto.PhoneNumber
        };

        return await _clientRepository.UpdateIndividualAsync(individualToUpdate);
    }

    public async Task<Company?> UpdateCompanyAsync(UpdateCompanyDto updateCompanyDto)
    {
        // Check if company exists
        var existingCompany = await _clientRepository.GetCompanyByKrsAsync(updateCompanyDto.Krs);
        if (existingCompany == null)
        {
            throw new KeyNotFoundException($"Company with KRS {updateCompanyDto.Krs} not found.");
        }

        var companyToUpdate = new Company
        {
            Krs = updateCompanyDto.Krs,
            Name = updateCompanyDto.Name,
            Address = updateCompanyDto.Address,
            Email = updateCompanyDto.Email,
            PhoneNumber = updateCompanyDto.PhoneNumber
        };

        return await _clientRepository.UpdateCompanyAsync(companyToUpdate);
    }

    public async Task<bool> DeleteIndividualAsync(string pesel)
    {
        // Check if individual exists
        var existingIndividual = await _clientRepository.GetIndividualByPeselAsync(pesel);
        if (existingIndividual == null)
        {
            throw new KeyNotFoundException($"Individual with PESEL {pesel} not found.");
        }

        return await _clientRepository.SoftDeleteIndividualAsync(pesel);
    }
}