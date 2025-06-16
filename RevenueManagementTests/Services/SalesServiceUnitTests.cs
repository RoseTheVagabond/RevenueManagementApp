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
        // Arrange
        var discountDto = new DiscountDTO
        {
            Percentage = 10,
            Start = DateTime.UtcNow.AddDays(1),
            End = DateTime.UtcNow.AddDays(30)
        };

        // Act
        var result = await _salesService.CreateDiscountAsync(discountDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(discountDto.Percentage, result.Percentage);
        Assert.Equal(discountDto.Start, result.Start);
        Assert.Equal(discountDto.End, result.End);
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task CreateDiscountAsync_ShouldThrowException_WhenStartDateIsAfterEndDate()
    {
        // Arrange
        var discountDto = new DiscountDTO
        {
            Percentage = 10,
            Start = DateTime.UtcNow.AddDays(30),
            End = DateTime.UtcNow.AddDays(1)
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _salesService.CreateDiscountAsync(discountDto));
        
        Assert.Contains("Start date must be before end date", exception.Message);
    }

    [Fact]
    public async Task CreateDiscountAsync_ShouldThrowException_WhenStartDateIsInPast()
    {
        // Arrange
        var discountDto = new DiscountDTO
        {
            Percentage = 10,
            Start = DateTime.UtcNow.AddDays(-1),
            End = DateTime.UtcNow.AddDays(30)
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _salesService.CreateDiscountAsync(discountDto));
        
        Assert.Contains("Start date cannot be in the past", exception.Message);
    }

    [Fact]
    public async Task CreateContractAsync_ShouldCreateContract_WhenValidIndividualDataProvided()
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

        // Act
        var result = await _salesService.CreateContractAsync(contractDto);

        // Assert
        Assert.True(result > 0);
    }

    [Fact]
    public async Task CreateContractAsync_ShouldCreateContract_WhenValidCompanyDataProvided()
    {
        // Arrange
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

        // Act
        var result = await _salesService.CreateContractAsync(contractDto);

        // Assert
        Assert.True(result > 0);
    }

    [Fact]
    public async Task CreateContractAsync_ShouldThrowException_WhenBothPeselAndKrsProvided()
    {
        // Arrange
        var contractDto = new ContractDTO
        {
            IndividualPesel = "12345678901",
            CompanyKrs = "1234567890",
            SoftwareId = 1,
            Start = DateTime.UtcNow.AddDays(1),
            End = DateTime.UtcNow.AddDays(10)
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _salesService.CreateContractAsync(contractDto));
        
        Assert.Contains("Cannot specify both Individual PESEL and Company KRS", exception.Message);
    }

    [Fact]
    public async Task CreateContractAsync_ShouldThrowException_WhenNeitherPeselNorKrsProvided()
    {
        // Arrange
        var contractDto = new ContractDTO
        {
            SoftwareId = 1,
            Start = DateTime.UtcNow.AddDays(1),
            End = DateTime.UtcNow.AddDays(10)
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _salesService.CreateContractAsync(contractDto));
        
        Assert.Contains("Either Individual PESEL or Company KRS must be provided", exception.Message);
    }

    [Fact]
    public async Task CreateContractAsync_ShouldThrowException_WhenClientDoesNotExist()
    {
        // Arrange
        var contractDto = new ContractDTO
        {
            IndividualPesel = "99999999999",
            SoftwareId = 1,
            Start = DateTime.UtcNow.AddDays(1),
            End = DateTime.UtcNow.AddDays(10)
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _salesService.CreateContractAsync(contractDto));
        
        Assert.Contains("Specified client does not exist", exception.Message);
    }

    [Fact]
    public async Task CreateContractAsync_ShouldThrowException_WhenSoftwareDoesNotExist()
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
        _fakeClientRepository.AddTestIndividual(individual);

        var contractDto = new ContractDTO
        {
            IndividualPesel = "12345678901",
            SoftwareId = 999,
            Start = DateTime.UtcNow.AddDays(1),
            End = DateTime.UtcNow.AddDays(10)
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _salesService.CreateContractAsync(contractDto));
        
        Assert.Contains("Specified software does not exist", exception.Message);
    }

    [Fact]
    public async Task CreateContractAsync_ShouldThrowException_WhenContractDurationTooShort()
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
            End = DateTime.UtcNow.AddDays(2) // Only 1 day, less than minimum 3 days
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _salesService.CreateContractAsync(contractDto));
        
        Assert.Contains("Contract duration must be between 3 and 30 days", exception.Message);
    }

    [Fact]
    public async Task CreateContractAsync_ShouldThrowException_WhenContractDurationTooLong()
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
            End = DateTime.UtcNow.AddDays(35) // More than maximum 30 days
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _salesService.CreateContractAsync(contractDto));
        
        Assert.Contains("Contract duration must be between 3 and 30 days", exception.Message);
    }

    [Fact]
    public async Task CreateContractAsync_ShouldThrowException_WhenClientHasActiveSubscription()
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

        // Add existing active contract
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

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _salesService.CreateContractAsync(contractDto));
        
        Assert.Contains("Client already has an active subscription for this software", exception.Message);
    }

    [Fact]
    public async Task CreateContractAsync_ShouldApplyDiscountCorrectly_WhenActiveDiscountExists()
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

        // Add active discount
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

        // Act
        var contractId = await _salesService.CreateContractAsync(contractDto);

        // Assert
        var createdContract = await _fakeSalesRepository.GetContractByIdAsync(contractId);
        Assert.NotNull(createdContract);
        
        // Expected: 1000 - (1000 * 0.20) = 800
        Assert.Equal(800m, createdContract.ToPay);
        Assert.Equal(discount.Id, createdContract.DiscountId);
    }

    [Fact]
    public async Task CreateContractAsync_ShouldCalculateCorrectPrice_WithAdditionalSupportYears()
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

        // Act
        var contractId = await _salesService.CreateContractAsync(contractDto);

        // Assert
        var createdContract = await _fakeSalesRepository.GetContractByIdAsync(contractId);
        Assert.NotNull(createdContract);
        
        // Expected: 1000 + (2 * 1000) = 3000
        Assert.Equal(3000m, createdContract.ToPay);
    }

    [Fact]
    public async Task DeleteContractAsync_ShouldDeleteContract_WhenContractExistsAndNotSigned()
    {
        // Arrange
        var contract = new Contract
        {
            Id = 1,
            IndividualPesel = "12345678901",
            SoftwareId = 1,
            Start = DateTime.UtcNow.AddDays(1),
            End = DateTime.UtcNow.AddDays(10),
            SoftwareDeadline = DateTime.UtcNow.AddYears(1),
            IsSigned = false,
            IsPaid = false,
            ToPay = 5000m,
            Paid = 0m
        };
        _fakeSalesRepository.AddTestContract(contract);

        // Act
        var result = await _salesService.DeleteContractAsync(1);

        // Assert
        Assert.True(result);
        
        // Verify contract is deleted
        var deletedContract = await _fakeSalesRepository.GetContractByIdAsync(1);
        Assert.Null(deletedContract);
    }

    [Fact]
    public async Task DeleteContractAsync_ShouldThrowException_WhenContractNotFound()
    {
        // Arrange
        var contractId = 999;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _salesService.DeleteContractAsync(contractId));
        
        Assert.Contains("Contract not found", exception.Message);
    }

    [Fact]
    public async Task DeleteContractAsync_ShouldThrowException_WhenContractIsSigned()
    {
        // Arrange
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

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _salesService.DeleteContractAsync(1));
        
        Assert.Contains("Cannot delete a signed contract", exception.Message);
    }

    [Fact]
    public async Task PayForContractAsync_ShouldProcessPayment_WhenValidPaymentProvided()
    {
        // Arrange
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
            Paid = 0m
        };
        _fakeSalesRepository.AddTestContract(contract);

        var paymentDto = new ContractPaymentDTO
        {
            ContractId = 1,
            PaymentAmount = 500m
        };

        // Act
        var result = await _salesService.PayForContractAsync(paymentDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(500m, result.Paid);
        Assert.False(result.IsPaid);
        Assert.False(result.IsSigned);
    }

    [Fact]
    public async Task PayForContractAsync_ShouldCompleteContract_WhenFullPaymentMade()
    {
        // Arrange
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

        // Act
        var result = await _salesService.PayForContractAsync(paymentDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1000m, result.Paid);
        Assert.True(result.IsPaid);
        Assert.True(result.IsSigned);
    }

    [Fact]
    public async Task PayForContractAsync_ShouldThrowException_WhenContractNotFound()
    {
        // Arrange
        var paymentDto = new ContractPaymentDTO
        {
            ContractId = 999,
            PaymentAmount = 500m
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _salesService.PayForContractAsync(paymentDto));
        
        Assert.Contains("Contract not found", exception.Message);
    }

    [Fact]
    public async Task PayForContractAsync_ShouldThrowException_WhenPaymentPeriodExpired()
    {
        // Arrange
        var contract = new Contract
        {
            Id = 1,
            IndividualPesel = "12345678901",
            SoftwareId = 1,
            Start = DateTime.UtcNow.AddDays(-10),
            End = DateTime.UtcNow.AddDays(-1), // Payment period expired
            SoftwareDeadline = DateTime.UtcNow.AddYears(1),
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

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _salesService.PayForContractAsync(paymentDto));
        
        Assert.Contains("Payment period for this contract has expired", exception.Message);
        Assert.Contains("Refund of 500PLN will be issued", exception.Message);
    }

    [Fact]
    public async Task PayForContractAsync_ShouldThrowException_WhenContractAlreadyPaid()
    {
        // Arrange
        var contract = new Contract
        {
            Id = 1,
            IndividualPesel = "12345678901",
            SoftwareId = 1,
            Start = DateTime.UtcNow.AddDays(-5),
            End = DateTime.UtcNow.AddDays(5),
            SoftwareDeadline = DateTime.UtcNow.AddYears(1),
            IsSigned = true,
            IsPaid = true,
            ToPay = 1000m,
            Paid = 1000m
        };
        _fakeSalesRepository.AddTestContract(contract);

        var paymentDto = new ContractPaymentDTO
        {
            ContractId = 1,
            PaymentAmount = 100m
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _salesService.PayForContractAsync(paymentDto));
        
        Assert.Contains("Contract is already fully paid", exception.Message);
    }

    [Fact]
    public async Task PayForContractAsync_ShouldThrowException_WhenPaymentExceedsRemainingBalance()
    {
        // Arrange
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
            Paid = 800m
        };
        _fakeSalesRepository.AddTestContract(contract);

        var paymentDto = new ContractPaymentDTO
        {
            ContractId = 1,
            PaymentAmount = 300m // Remaining is only 200
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _salesService.PayForContractAsync(paymentDto));
        
        Assert.Contains("Payment amount exceeds remaining balance", exception.Message);
        Assert.Contains("to pay: 200 PLN", exception.Message);
    }

    [Fact]
    public async Task CalculateRevenueAsync_ShouldReturnCorrectRevenue_ForAllContracts()
    {
        // Arrange
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

        // Add paid contracts
        var paidContract1 = new Contract
        {
            Id = 1,
            SoftwareId = 1,
            IsPaid = true,
            IsSigned = true,
            ToPay = 1000m,
            Paid = 1000m,
            Start = DateTime.UtcNow.AddDays(-10),
            End = DateTime.UtcNow.AddDays(-5)
        };
        
        var paidContract2 = new Contract
        {
            Id = 2,
            SoftwareId = 1,
            IsPaid = true,
            IsSigned = true,
            ToPay = 1500m,
            Paid = 1500m,
            Start = DateTime.UtcNow.AddDays(-8),
            End = DateTime.UtcNow.AddDays(-3)
        };

        // Add unpaid contract
        var unpaidContract = new Contract
        {
            Id = 3,
            SoftwareId = 1,
            IsPaid = false,
            IsSigned = false,
            ToPay = 2000m,
            Paid = 0m,
            Start = DateTime.UtcNow.AddDays(1),
            End = DateTime.UtcNow.AddDays(10)
        };

        _fakeSalesRepository.AddTestContract(paidContract1);
        _fakeSalesRepository.AddTestContract(paidContract2);
        _fakeSalesRepository.AddTestContract(unpaidContract);

        var request = new RevenueRequestDTO
        {
            Currency = "PLN"
        };

        // Act
        var result = await _salesService.CalculateRevenueAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2500m, result.CurrentRevenue); // 1000 + 1500
        Assert.Equal(4500m, result.PredictedRevenue); // 2500 + 2000
        Assert.Equal("PLN", result.Currency);
        Assert.Equal(1.0m, result.ExchangeRate);
        Assert.Equal(3, result.TotalContracts);
        Assert.Equal(2, result.PaidContracts);
        Assert.Equal(1, result.UnpaidContracts);
    }

    [Fact]
    public async Task CalculateRevenueAsync_ShouldReturnCorrectRevenue_ForSpecificSoftware()
    {
        // Arrange
        var software1 = new Software
        {
            Id = 1,
            Name = "Software 1",
            Description = "Test Description",
            CurrentVersion = "1.0",
            Price = 1000m,
            CathegoryId = 1,
            Cathegory = new Cathegory { Id = 1, Name = "Business" }
        };

        var software2 = new Software
        {
            Id = 2,
            Name = "Software 2",
            Description = "Test Description",
            CurrentVersion = "1.0",
            Price = 2000m,
            CathegoryId = 1,
            Cathegory = new Cathegory { Id = 1, Name = "Business" }
        };

        _fakeSalesRepository.AddTestSoftware(software1);
        _fakeSalesRepository.AddTestSoftware(software2);

        // Add contracts for different software
        var contract1 = new Contract
        {
            Id = 1,
            SoftwareId = 1,
            IsPaid = true,
            IsSigned = true,
            ToPay = 1000m,
            Paid = 1000m,
            Start = DateTime.UtcNow.AddDays(-10),
            End = DateTime.UtcNow.AddDays(-5)
        };
        
        var contract2 = new Contract
        {
            Id = 2,
            SoftwareId = 2,
            IsPaid = true,
            IsSigned = true,
            ToPay = 2000m,
            Paid = 2000m,
            Start = DateTime.UtcNow.AddDays(-8),
            End = DateTime.UtcNow.AddDays(-3)
        };

        _fakeSalesRepository.AddTestContract(contract1);
        _fakeSalesRepository.AddTestContract(contract2);

        var request = new RevenueRequestDTO
        {
            SoftwareId = 1,
            Currency = "PLN"
        };

        // Act
        var result = await _salesService.CalculateRevenueAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1000m, result.CurrentRevenue); // Only software 1
        Assert.Equal(1000m, result.PredictedRevenue); // Only software 1
        Assert.Equal("Software 1", result.SoftwareName);
        Assert.Equal(1, result.TotalContracts);
        Assert.Equal(1, result.PaidContracts);
        Assert.Equal(0, result.UnpaidContracts);
    }
}