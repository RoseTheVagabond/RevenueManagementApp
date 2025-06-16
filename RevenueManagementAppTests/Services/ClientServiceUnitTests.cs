using RevenueManagementApp.DTOs;
using RevenueManagementApp.Models;
using RevenueManagementApp.Services;
using RevenueManagementApp.Tests.Fakes;
using Xunit;

namespace RevenueManagementAppTests.Services;

public class ClientServiceUnitTests
{
    private readonly FakeClientRepository _fakeClientRepository;
    private readonly ClientService _clientService;

    public ClientServiceUnitTests()
    {
        _fakeClientRepository = new FakeClientRepository();
        _clientService = new ClientService(_fakeClientRepository);
    }

    [Fact]
    public async Task AddIndividualAsync_ShouldAddIndividual_WhenValidDataProvided()
    {
        // Arrange
        var createIndividualDto = new CreateIndividualDto
        {
            Pesel = "12345678901",
            FirstName = "John",
            LastName = "Doe",
            Address = "123 Main St",
            Email = "john.doe@example.com",
            PhoneNumber = "123456789"
        };

        // Act
        var result = await _clientService.AddIndividualAsync(createIndividualDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createIndividualDto.Pesel, result.Pesel);
        Assert.Equal(createIndividualDto.FirstName, result.FirstName);
        Assert.Equal(createIndividualDto.LastName, result.LastName);
        Assert.Equal(createIndividualDto.Address, result.Address);
        Assert.Equal(createIndividualDto.Email, result.Email);
        Assert.Equal(createIndividualDto.PhoneNumber, result.PhoneNumber);
    }

    [Fact]
    public async Task AddIndividualAsync_ShouldThrowException_WhenIndividualAlreadyExists()
    {
        // Arrange
        var existingIndividual = new Individual
        {
            Pesel = "12345678901",
            FirstName = "Existing",
            LastName = "User",
            Address = "456 Oak St",
            Email = "existing@example.com",
            PhoneNumber = "987654321"
        };
        _fakeClientRepository.AddTestIndividual(existingIndividual);

        var createIndividualDto = new CreateIndividualDto
        {
            Pesel = "12345678901",
            FirstName = "John",
            LastName = "Doe",
            Address = "123 Main St",
            Email = "john.doe@example.com",
            PhoneNumber = "123456789"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _clientService.AddIndividualAsync(createIndividualDto));
        
        Assert.Contains("Individual with PESEL 12345678901 already exists", exception.Message);
    }

    [Fact]
    public async Task AddCompanyAsync_ShouldAddCompany_WhenValidDataProvided()
    {
        // Arrange
        var createCompanyDto = new CreateCompanyDto
        {
            Krs = "1234567890",
            Name = "Test Company",
            Address = "123 Business St",
            Email = "test@company.com",
            PhoneNumber = "123456789"
        };

        // Act
        var result = await _clientService.AddCompanyAsync(createCompanyDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createCompanyDto.Krs, result.Krs);
        Assert.Equal(createCompanyDto.Name, result.Name);
        Assert.Equal(createCompanyDto.Address, result.Address);
        Assert.Equal(createCompanyDto.Email, result.Email);
        Assert.Equal(createCompanyDto.PhoneNumber, result.PhoneNumber);
    }

    [Fact]
    public async Task AddCompanyAsync_ShouldThrowException_WhenCompanyAlreadyExists()
    {
        // Arrange
        var existingCompany = new Company
        {
            Krs = "1234567890",
            Name = "Existing Company",
            Address = "456 Business Ave",
            Email = "existing@company.com",
            PhoneNumber = "987654321"
        };
        _fakeClientRepository.AddTestCompany(existingCompany);

        var createCompanyDto = new CreateCompanyDto
        {
            Krs = "1234567890",
            Name = "Test Company",
            Address = "123 Business St",
            Email = "test@company.com",
            PhoneNumber = "123456789"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _clientService.AddCompanyAsync(createCompanyDto));
        
        Assert.Contains("Company with KRS 1234567890 already exists", exception.Message);
    }

    [Fact]
    public async Task UpdateIndividualAsync_ShouldUpdateIndividual_WhenValidDataProvided()
    {
        // Arrange
        var existingIndividual = new Individual
        {
            Pesel = "12345678901",
            FirstName = "John",
            LastName = "Doe",
            Address = "123 Main St",
            Email = "john.doe@example.com",
            PhoneNumber = "123456789"
        };
        _fakeClientRepository.AddTestIndividual(existingIndividual);

        var updateIndividualDto = new UpdateIndividualDto
        {
            Pesel = "12345678901",
            FirstName = "Johnny",
            LastName = "Smith",
            Address = "456 New St",
            Email = "johnny.smith@example.com",
            PhoneNumber = "987654321"
        };

        // Act
        var result = await _clientService.UpdateIndividualAsync(updateIndividualDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updateIndividualDto.FirstName, result.FirstName);
        Assert.Equal(updateIndividualDto.LastName, result.LastName);
        Assert.Equal(updateIndividualDto.Address, result.Address);
        Assert.Equal(updateIndividualDto.Email, result.Email);
        Assert.Equal(updateIndividualDto.PhoneNumber, result.PhoneNumber);
    }

    [Fact]
    public async Task UpdateIndividualAsync_ShouldThrowException_WhenIndividualNotFound()
    {
        // Arrange
        var updateIndividualDto = new UpdateIndividualDto
        {
            Pesel = "99999999999",
            FirstName = "Johnny",
            LastName = "Smith",
            Address = "456 New St",
            Email = "johnny.smith@example.com",
            PhoneNumber = "987654321"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _clientService.UpdateIndividualAsync(updateIndividualDto));
        
        Assert.Contains("Individual with PESEL 99999999999 not found", exception.Message);
    }

    [Fact]
    public async Task UpdateCompanyAsync_ShouldUpdateCompany_WhenValidDataProvided()
    {
        // Arrange
        var existingCompany = new Company
        {
            Krs = "1234567890",
            Name = "Test Company",
            Address = "123 Business St",
            Email = "test@company.com",
            PhoneNumber = "123456789"
        };
        _fakeClientRepository.AddTestCompany(existingCompany);

        var updateCompanyDto = new UpdateCompanyDto
        {
            Krs = "1234567890",
            Name = "Updated Company",
            Address = "456 New Business Ave",
            Email = "updated@company.com",
            PhoneNumber = "987654321"
        };

        // Act
        var result = await _clientService.UpdateCompanyAsync(updateCompanyDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updateCompanyDto.Name, result.Name);
        Assert.Equal(updateCompanyDto.Address, result.Address);
        Assert.Equal(updateCompanyDto.Email, result.Email);
        Assert.Equal(updateCompanyDto.PhoneNumber, result.PhoneNumber);
    }

    [Fact]
    public async Task UpdateCompanyAsync_ShouldThrowException_WhenCompanyNotFound()
    {
        // Arrange
        var updateCompanyDto = new UpdateCompanyDto
        {
            Krs = "9999999999",
            Name = "Updated Company",
            Address = "456 New Business Ave",
            Email = "updated@company.com",
            PhoneNumber = "987654321"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _clientService.UpdateCompanyAsync(updateCompanyDto));
        
        Assert.Contains("Company with KRS 9999999999 not found", exception.Message);
    }

    [Fact]
    public async Task DeleteIndividualAsync_ShouldSoftDeleteIndividual_WhenIndividualExists()
    {
        // Arrange
        var existingIndividual = new Individual
        {
            Pesel = "12345678901",
            FirstName = "John",
            LastName = "Doe",
            Address = "123 Main St",
            Email = "john.doe@example.com",
            PhoneNumber = "123456789"
        };
        _fakeClientRepository.AddTestIndividual(existingIndividual);

        // Act
        var result = await _clientService.DeleteIndividualAsync("12345678901");

        // Assert
        Assert.True(result);
        
        // Verify the individual is soft deleted
        var deletedIndividual = await _fakeClientRepository.GetIndividualByPeselAsync("12345678901");
        Assert.Null(deletedIndividual); // Should return null because DeletedAt is set
    }

    [Fact]
    public async Task DeleteIndividualAsync_ShouldThrowException_WhenIndividualNotFound()
    {
        // Arrange
        var pesel = "99999999999";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _clientService.DeleteIndividualAsync(pesel));
        
        Assert.Contains("Individual with PESEL 99999999999 not found", exception.Message);
    }

    [Fact]
    public async Task GetAllClientsAsync_ShouldReturnAllClientsData_WhenClientsExist()
    {
        // Arrange
        var individual = new Individual
        {
            Pesel = "12345678901",
            FirstName = "John",
            LastName = "Doe",
            Address = "123 Main St",
            Email = "john.doe@example.com",
            PhoneNumber = "123456789"
        };
        
        var company = new Company
        {
            Krs = "1234567890",
            Name = "Test Company",
            Address = "123 Business St",
            Email = "test@company.com",
            PhoneNumber = "123456789"
        };

        _fakeClientRepository.AddTestIndividual(individual);
        _fakeClientRepository.AddTestCompany(company);

        // Act
        var result = await _clientService.GetAllClientsAsync();

        // Assert
        Assert.NotNull(result);
        var resultObj = (dynamic)result;
        Assert.Equal(1, ((List<Individual>)resultObj.Individuals).Count);
        Assert.Equal(1, ((List<Company>)resultObj.Companies).Count);
        Assert.Equal(2, (int)resultObj.TotalClients);
    }
}