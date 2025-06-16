using RevenueManagementApp.DTOs;
using RevenueManagementApp.Models;
using RevenueManagementApp.Services;
using RevenueManagementApp.Tests.Fakes;
using Xunit;

namespace RevenueManagementApp.Tests.Services;

public class SalesServiceUnitTests
{
    private readonly FakeSalesRepository _fakeSalesRepository;
    private readonly FakeClientRepository _fakeClientRepository;
    private readonly SalesService _salesService;

    public SalesServiceUnitTests()
    {
        _fakeSalesRepository = new FakeSalesRepository();
        _fakeClientRepository = new FakeClientRepository();
        _salesService = new SalesService(_fakeSalesRepository, _fakeClientRepository);
    }

    [Fact]
    public async Task CreateDiscountAsync_ShouldCreateDiscount_WhenValidDataProvided()
    {
        var discountDto = new DiscountDTO
        {
            Percentage = 10,
            Start = DateTime.UtcNow.AddDays(1),
            End = DateTime.UtcNow.AddDays(30)
        };
        
        var result = await _salesService.CreateDiscountAsync(discountDto);
        
        Assert.NotNull(result);
        Assert.Equal(discountDto.Percentage, result.Percentage);
        Assert.Equal(discountDto.Start, result.Start);
        Assert.Equal(discountDto.End, result.End);
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task CreateDiscountAsync_ShouldThrowException_WhenStartDateIsAfterEndDate()
    {
        var discountDto = new DiscountDTO
        {
            Percentage = 10,
            Start = DateTime.UtcNow.AddDays(30),
            End = DateTime.UtcNow.AddDays(1)
        };
        
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _salesService.CreateDiscountAsync(discountDto));
        
        Assert.Contains("Start date must be before end date", exception.Message);
    }

    [Fact]
    public async Task CreateContractAsync_ShouldCreateContract_WhenValidIndividualDataProvided()
    {
        var individual = new Individual
        {
            Pesel = "12345678901",
            FirstName = "John",
            LastName = "Doe",
            Address = "123 Main St",
            Email = "john.doe@example.com",
            PhoneNumber = "123456789"
        };
        _fakeClientRepository.AddTestIndividual(individual);

        var software = new Software
        {
            Id = 1,
            Name = "Test Software",
            Description = "Test Description",
            CurrentVersion = "1.0",
            Price = 5000m,
            CathegoryId = 1,
            Cathegory = new Cathegory { Id = 1, Name = "Business" }
        };
        _fakeSalesRepository.AddTestSoftware(software);

        var contractDto = new ContractDTO
        {
            IndividualPesel = "12345678901",
            SoftwareId = 1,
            Start = DateTime.UtcNow.AddDays(1),
            End = DateTime.UtcNow.AddDays(10),
            AdditionalSupportYears = 1
        };
        
        var result = await _salesService.CreateContractAsync(contractDto);
        
        Assert.True(result > 0);
    }

    [Fact]
    public async Task CreateContractAsync_ShouldCreateContract_WhenValidCompanyDataProvided()
    {
        var company = new Company
        {
            Krs = "1234567890",
            Name = "Test Company",
            Address = "123 Business St",
            Email = "test@company.com",
            PhoneNumber = "123456789"
        };
        _fakeClientRepository.AddTestCompany(company);

        var software = new Software
        {
            Id = 1,
            Name = "Test Software",
            Description = "Test Description",
            CurrentVersion = "1.0",
            Price = 5000m,
            CathegoryId = 1,
            Cathegory = new Cathegory { Id = 1, Name = "Business" }
        };
        _fakeSalesRepository.AddTestSoftware(software);

        var contractDto = new ContractDTO
        {
            CompanyKrs = "1234567890",
            SoftwareId = 1,
            Start = DateTime.UtcNow.AddDays(1),
            End = DateTime.UtcNow.AddDays(10),
            AdditionalSupportYears = 2
        };
        
        var result = await _salesService.CreateContractAsync(contractDto);
        
        Assert.True(result > 0);
    }

    [Fact]
    public async Task CreateContractAsync_ShouldThrowException_WhenBothPeselAndKrsProvided()
    {
        var contractDto = new ContractDTO
        {
            IndividualPesel = "12345678901",
            CompanyKrs = "1234567890",
            SoftwareId = 1,
            Start = DateTime.UtcNow.AddDays(1),
            End = DateTime.UtcNow.AddDays(10)
        };
        
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _salesService.CreateContractAsync(contractDto));
        
        Assert.Contains("Cannot specify both Individual PESEL and Company KRS", exception.Message);
    }

    [Fact]
    public async Task CreateContractAsync_ShouldThrowException_WhenClientDoesNotExist()
    {
        var contractDto = new ContractDTO
        {
            IndividualPesel = "99999999999",
            SoftwareId = 1,
            Start = DateTime.UtcNow.AddDays(1),
            End = DateTime.UtcNow.AddDays(10)
        };
        
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _salesService.CreateContractAsync(contractDto));
        
        Assert.Contains("Specified client does not exist", exception.Message);
    }

    [Fact]
    public async Task CreateContractAsync_ShouldThrowException_WhenSoftwareDoesNotExist()
    {
        var individual = new Individual
        {
            Pesel = "12345678901",
            FirstName = "John",
            LastName = "Doe",
            Address = "123 Main St",
            Email = "john.doe@example.com",
            PhoneNumber = "123456789"
        };
        _fakeClientRepository.AddTestIndividual(individual);

        var contractDto = new ContractDTO
        {
            IndividualPesel = "12345678901",
            SoftwareId = 999,
            Start = DateTime.UtcNow.AddDays(1),
            End = DateTime.UtcNow.AddDays(10)
        };
        
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _salesService.CreateContractAsync(contractDto));
        
        Assert.Contains("Specified software does not exist", exception.Message);
    }

    [Fact]
    public async Task CreateContractAsync_ShouldThrowException_WhenContractDurationTooShort()
    {
        var individual = new Individual
        {
            Pesel = "12345678901",
            FirstName = "John",
            LastName = "Doe",
            Address = "123 Main St",
            Email = "john.doe@example.com",
            PhoneNumber = "123456789"
        };
        _fakeClientRepository.AddTestIndividual(individual);

        var software = new Software
        {
            Id = 1,
            Name = "Test Software",
            Description = "Test Description",
            CurrentVersion = "1.0",
            Price = 5000m,
            CathegoryId = 1,
            Cathegory = new Cathegory { Id = 1, Name = "Business" }
        };
        _fakeSalesRepository.AddTestSoftware(software);

        var contractDto = new ContractDTO
        {
            IndividualPesel = "12345678901",
            SoftwareId = 1,
            Start = DateTime.Now.AddDays(1),
            End = DateTime.Now.AddDays(2)
        };
        
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _salesService.CreateContractAsync(contractDto));
        
        Assert.Contains("Contract duration must be between 3 and 30 days", exception.Message);
    }

    [Fact]
    public async Task CreateContractAsync_ShouldThrowException_WhenContractDurationTooLong()
    {
        var individual = new Individual
        {
            Pesel = "12345678901",
            FirstName = "John",
            LastName = "Doe",
            Address = "123 Main St",
            Email = "john.doe@example.com",
            PhoneNumber = "123456789"
        };
        _fakeClientRepository.AddTestIndividual(individual);

        var software = new Software
        {
            Id = 1,
            Name = "Test Software",
            Description = "Test Description",
            CurrentVersion = "1.0",
            Price = 5000m,
            CathegoryId = 1,
            Cathegory = new Cathegory { Id = 1, Name = "Business" }
        };
        _fakeSalesRepository.AddTestSoftware(software);

        var contractDto = new ContractDTO
        {
            IndividualPesel = "12345678901",
            SoftwareId = 1,
            Start = DateTime.Now.AddDays(1),
            End = DateTime.Now.AddDays(35)
        };
        
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _salesService.CreateContractAsync(contractDto));
        
        Assert.Contains("Contract duration must be between 3 and 30 days", exception.Message);
    }

    [Fact]
    public async Task CreateContractAsync_ShouldThrowException_WhenClientHasActiveSubscription()
    {
        var individual = new Individual
        {
            Pesel = "12345678901",
            FirstName = "John",
            LastName = "Doe",
            Address = "123 Main St",
            Email = "john.doe@example.com",
            PhoneNumber = "123456789"
        };
        _fakeClientRepository.AddTestIndividual(individual);

        var software = new Software
        {
            Id = 1,
            Name = "Test Software",
            Description = "Test Description",
            CurrentVersion = "1.0",
            Price = 5000m,
            CathegoryId = 1,
            Cathegory = new Cathegory { Id = 1, Name = "Business" }
        };
        _fakeSalesRepository.AddTestSoftware(software);
        
        var existingContract = new Contract
        {
            Id = 1,
            IndividualPesel = "12345678901",
            SoftwareId = 1,
            Start = DateTime.UtcNow.AddDays(-5),
            End = DateTime.UtcNow.AddDays(5),
            SoftwareDeadline = DateTime.UtcNow.AddYears(1),
            IsSigned = true,
            IsPaid = true,
            ToPay = 5000m,
            Paid = 5000m
        };
        _fakeSalesRepository.AddTestContract(existingContract);

        var contractDto = new ContractDTO
        {
            IndividualPesel = "12345678901",
            SoftwareId = 1,
            Start = DateTime.UtcNow.AddDays(1),
            End = DateTime.UtcNow.AddDays(10)
        };
        
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _salesService.CreateContractAsync(contractDto));
        
        Assert.Contains("Client already has an active subscription for this software", exception.Message);
    }

    [Fact]
    public async Task CreateContractAsync_ShouldApplyDiscountCorrectly_WhenActiveDiscountExists()
    {
        var individual = new Individual
        {
            Pesel = "12345678901",
            FirstName = "John",
            LastName = "Doe",
            Address = "123 Main St",
            Email = "john.doe@example.com",
            PhoneNumber = "123456789"
        };
        _fakeClientRepository.AddTestIndividual(individual);

        var software = new Software
        {
            Id = 1,
            Name = "Test Software",
            Description = "Test Description",
            CurrentVersion = "1.0",
            Price = 1000m,
            CathegoryId = 1,
            Cathegory = new Cathegory { Id = 1, Name = "Business" }
        };
        _fakeSalesRepository.AddTestSoftware(software);
        
        var discount = new Discount
        {
            Id = 1,
            Percentage = 20,
            Start = DateTime.UtcNow.AddDays(-1),
            End = DateTime.UtcNow.AddDays(10)
        };
        _fakeSalesRepository.AddTestDiscount(discount);

        var contractDto = new ContractDTO
        {
            IndividualPesel = "12345678901",
            SoftwareId = 1,
            Start = DateTime.UtcNow.AddDays(1),
            End = DateTime.UtcNow.AddDays(10),
            AdditionalSupportYears = 0
        };
        
        var contractId = await _salesService.CreateContractAsync(contractDto);
        
        var createdContract = await _fakeSalesRepository.GetContractByIdAsync(contractId);
        Assert.NotNull(createdContract);
        
        Assert.Equal(800m, createdContract.ToPay);
        Assert.Equal(discount.Id, createdContract.DiscountId);
    }

    [Fact]
    public async Task CreateContractAsync_ShouldCalculateCorrectPrice_WithAdditionalSupportYears()
    {
        var individual = new Individual
        {
            Pesel = "12345678901",
            FirstName = "John",
            LastName = "Doe",
            Address = "123 Main St",
            Email = "john.doe@example.com",
            PhoneNumber = "123456789"
        };
        _fakeClientRepository.AddTestIndividual(individual);

        var software = new Software
        {
            Id = 1,
            Name = "Test Software",
            Description = "Test Description",
            CurrentVersion = "1.0",
            Price = 1000m,
            CathegoryId = 1,
            Cathegory = new Cathegory { Id = 1, Name = "Business" }
        };
        _fakeSalesRepository.AddTestSoftware(software);

        var contractDto = new ContractDTO
        {
            IndividualPesel = "12345678901",
            SoftwareId = 1,
            Start = DateTime.UtcNow.AddDays(1),
            End = DateTime.UtcNow.AddDays(10),
            AdditionalSupportYears = 2
        };
        
        var contractId = await _salesService.CreateContractAsync(contractDto);
        
        var createdContract = await _fakeSalesRepository.GetContractByIdAsync(contractId);
        Assert.NotNull(createdContract);
        
        Assert.Equal(3000m, createdContract.ToPay);
    }

    [Fact]
    public async Task DeleteContractAsync_ShouldThrowException_WhenContractIsSigned()
    {
        var contract = new Contract
        {
            Id = 1,
            IndividualPesel = "12345678901",
            SoftwareId = 1,
            Start = DateTime.UtcNow.AddDays(1),
            End = DateTime.UtcNow.AddDays(10),
            SoftwareDeadline = DateTime.UtcNow.AddYears(1),
            IsSigned = true,
            IsPaid = true,
            ToPay = 5000m,
            Paid = 5000m
        };
        _fakeSalesRepository.AddTestContract(contract);
        
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _salesService.DeleteContractAsync(1));
        
        Assert.Contains("Cannot delete a signed contract", exception.Message);
    }

    [Fact]
    public async Task PayForContractAsync_ShouldCompleteContract_WhenFullPaymentMade()
    {
        var contract = new Contract
        {
            Id = 1,
            IndividualPesel = "12345678901",
            SoftwareId = 1,
            Start = DateTime.UtcNow.AddDays(-5),
            End = DateTime.UtcNow.AddDays(5),
            SoftwareDeadline = DateTime.UtcNow.AddYears(1),
            IsSigned = false,
            IsPaid = false,
            ToPay = 1000m,
            Paid = 600m
        };
        _fakeSalesRepository.AddTestContract(contract);

        var paymentDto = new ContractPaymentDTO
        {
            ContractId = 1,
            PaymentAmount = 400m
        };
        
        var result = await _salesService.PayForContractAsync(paymentDto);
        
        Assert.NotNull(result);
        Assert.Equal(1000m, result.Paid);
        Assert.True(result.IsPaid);
        Assert.True(result.IsSigned);
    }

    [Fact]
    public async Task PayForContractAsync_ShouldThrowException_WhenPaymentPeriodExpired()
    {
        var contract = new Contract
        {
            Id = 1,
            IndividualPesel = "12345678901",
            SoftwareId = 1,
            Start = DateTime.Now.AddDays(-10),
            End = DateTime.Now.AddDays(-1), // Payment period expired
            SoftwareDeadline = DateTime.Now.AddYears(1),
            IsSigned = false,
            IsPaid = false,
            ToPay = 1000m,
            Paid = 500m
        };
        _fakeSalesRepository.AddTestContract(contract);

        var paymentDto = new ContractPaymentDTO
        {
            ContractId = 1,
            PaymentAmount = 500m
        };
        
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _salesService.PayForContractAsync(paymentDto));
        
        Assert.Contains("Payment period for this contract has expired", exception.Message);
        Assert.Contains("Refund of 500PLN will be issued", exception.Message);
    }
}