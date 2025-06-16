using RevenueManagementApp.DTOs;
using RevenueManagementApp.Models;
using RevenueManagementApp.Services;
using RevenueManagementApp.Tests.Fakes;
using Xunit;

namespace RevenueManagementApp.Tests.Revenue;

public class RevenueCalculationUnitTests
{
    private readonly FakeSalesRepository _fakeSalesRepository;
    private readonly FakeClientRepository _fakeClientRepository;
    private readonly SalesService _salesService;

    public RevenueCalculationUnitTests()
    {
        _fakeSalesRepository = new FakeSalesRepository();
        _fakeClientRepository = new FakeClientRepository();
        _salesService = new SalesService(_fakeSalesRepository, _fakeClientRepository);
    }

    [Fact]
    public async Task CalculateRevenueAsync_ShouldReturnZeroRevenue_WhenNoContracts()
    {
        // Arrange
        var request = new RevenueRequestDTO
        {
            Currency = "PLN"
        };

        // Act
        var result = await _salesService.CalculateRevenueAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0m, result.CurrentRevenue);
        Assert.Equal(0m, result.PredictedRevenue);
        Assert.Equal("PLN", result.Currency);
        Assert.Equal(1.0m, result.ExchangeRate);
        Assert.Equal(0, result.TotalContracts);
        Assert.Equal(0, result.PaidContracts);
        Assert.Equal(0, result.UnpaidContracts);
        Assert.Null(result.SoftwareName);
    }

    [Fact]
    public async Task CalculateRevenueAsync_ShouldIncludeUnpaidContractsInPaymentPeriod_InPredictedRevenue()
    {
        // Arrange
        var paidContract = new Contract
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

        var unpaidContractInPaymentPeriod = new Contract
        {
            Id = 2,
            SoftwareId = 1,
            IsPaid = false,
            IsSigned = false,
            ToPay = 2000m,
            Paid = 0m,
            Start = DateTime.UtcNow.AddDays(1),
            End = DateTime.UtcNow.AddDays(10) // Still in payment period
        };

        var unpaidContractExpired = new Contract
        {
            Id = 3,
            SoftwareId = 1,
            IsPaid = false,
            IsSigned = false,
            ToPay = 1500m,
            Paid = 0m,
            Start = DateTime.UtcNow.AddDays(-20),
            End = DateTime.UtcNow.AddDays(-10) // Payment period expired
        };

        _fakeSalesRepository.AddTestContract(paidContract);
        _fakeSalesRepository.AddTestContract(unpaidContractInPaymentPeriod);
        _fakeSalesRepository.AddTestContract(unpaidContractExpired);

        var request = new RevenueRequestDTO { Currency = "PLN" };

        // Act
        var result = await _salesService.CalculateRevenueAsync(request);

        // Assert
        Assert.Equal(1000m, result.CurrentRevenue); // Only paid contract
        Assert.Equal(3000m, result.PredictedRevenue); // Paid contract + unpaid contract in payment period
    }

    [Fact]
    public async Task CalculateRevenueAsync_ShouldFilterBySpecificSoftware_WhenSoftwareIdProvided()
    {
        // Arrange
        var software1 = new Software
        {
            Id = 1,
            Name = "Software One",
            Price = 1000m,
            CathegoryId = 1,
            Cathegory = new Cathegory { Id = 1, Name = "Business" }
        };

        var software2 = new Software
        {
            Id = 2,
            Name = "Software Two",
            Price = 2000m,
            CathegoryId = 1,
            Cathegory = new Cathegory { Id = 1, Name = "Business" }
        };

        _fakeSalesRepository.AddTestSoftware(software1);
        _fakeSalesRepository.AddTestSoftware(software2);

        var contractSoftware1 = new Contract
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

        var contractSoftware2 = new Contract
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

        var anotherContractSoftware1 = new Contract
        {
            Id = 3,
            SoftwareId = 1,
            IsPaid = false,
            IsSigned = false,
            ToPay = 1500m,
            Paid = 0m,
            Start = DateTime.UtcNow.AddDays(1),
            End = DateTime.UtcNow.AddDays(10)
        };

        _fakeSalesRepository.AddTestContract(contractSoftware1);
        _fakeSalesRepository.AddTestContract(contractSoftware2);
        _fakeSalesRepository.AddTestContract(anotherContractSoftware1);

        var request = new RevenueRequestDTO
        {
            SoftwareId = 1,
            Currency = "PLN"
        };

        // Act
        var result = await _salesService.CalculateRevenueAsync(request);

        // Assert
        Assert.Equal(1000m, result.CurrentRevenue); // Only software 1 paid contracts
        Assert.Equal(2500m, result.PredictedRevenue); // Software 1 paid + unpaid contracts
        Assert.Equal("Software One", result.SoftwareName);
        Assert.Equal(2, result.TotalContracts); // Only software 1 contracts
        Assert.Equal(1, result.PaidContracts);
        Assert.Equal(1, result.UnpaidContracts);
    }

    [Fact]
    public async Task CalculateRevenueAsync_ShouldHandleNonExistentSoftware_WhenSoftwareIdProvided()
    {
        // Arrange
        var contract = new Contract
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
        _fakeSalesRepository.AddTestContract(contract);

        var request = new RevenueRequestDTO
        {
            SoftwareId = 999, // Non-existent software
            Currency = "PLN"
        };

        // Act
        var result = await _salesService.CalculateRevenueAsync(request);

        // Assert
        Assert.Equal(0m, result.CurrentRevenue);
        Assert.Equal(0m, result.PredictedRevenue);
        Assert.Null(result.SoftwareName); // No software found
        Assert.Equal(0, result.TotalContracts);
        Assert.Equal(0, result.PaidContracts);
        Assert.Equal(0, result.UnpaidContracts);
    }

    [Fact]
    public async Task CalculateRevenueAsync_ShouldReturnPLNCurrency_WhenNoCurrencySpecified()
    {
        // Arrange
        var contract = new Contract
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
        _fakeSalesRepository.AddTestContract(contract);

        var request = new RevenueRequestDTO(); // No currency specified

        // Act
        var result = await _salesService.CalculateRevenueAsync(request);

        // Assert
        Assert.Equal("PLN", result.Currency);
        Assert.Equal(1.0m, result.ExchangeRate);
        Assert.Equal(1000m, result.CurrentRevenue);
        Assert.Equal(1000m, result.PredictedRevenue);
    }

    [Fact]
    public async Task CalculateRevenueAsync_ShouldReturnPLNCurrency_WhenPLNSpecified()
    {
        // Arrange
        var contract = new Contract
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
        _fakeSalesRepository.AddTestContract(contract);

        var request = new RevenueRequestDTO
        {
            Currency = "PLN"
        };

        // Act
        var result = await _salesService.CalculateRevenueAsync(request);

        // Assert
        Assert.Equal("PLN", result.Currency);
        Assert.Equal(1.0m, result.ExchangeRate);
        Assert.Equal(1000m, result.CurrentRevenue);
        Assert.Equal(1000m, result.PredictedRevenue);
    }

    [Fact]
    public async Task GetExchangeRateAsync_ShouldThrowException_WhenUnsupportedCurrency()
    {
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _salesService.GetExchangeRateAsync("INVALID"));
        
        Assert.Contains("Currency INVALID not supported", exception.Message);
    }

    [Fact]
    public async Task CalculateRevenueAsync_ShouldCalculateCorrectRevenue_WithComplexScenario()
    {
        // Arrange - Complex scenario with multiple contracts in different states
        var software = new Software
        {
            Id = 1,
            Name = "Enterprise Software",
            Price = 10000m,
            CathegoryId = 1,
            Cathegory = new Cathegory { Id = 1, Name = "Business" }
        };
        _fakeSalesRepository.AddTestSoftware(software);

        // Fully paid contracts (should count in current revenue)
        var paidContract1 = new Contract
        {
            Id = 1,
            SoftwareId = 1,
            IsPaid = true,
            IsSigned = true,
            ToPay = 12000m,
            Paid = 12000m,
            Start = DateTime.UtcNow.AddDays(-30),
            End = DateTime.UtcNow.AddDays(-25)
        };

        var paidContract2 = new Contract
        {
            Id = 2,
            SoftwareId = 1,
            IsPaid = true,
            IsSigned = true,
            ToPay = 15000m,
            Paid = 15000m,
            Start = DateTime.UtcNow.AddDays(-20),
            End = DateTime.UtcNow.AddDays(-15)
        };

        // Partially paid contract still in payment period (should count unpaid amount in predicted)
        var partiallyPaidContract = new Contract
        {
            Id = 3,
            SoftwareId = 1,
            IsPaid = false,
            IsSigned = false,
            ToPay = 20000m,
            Paid = 8000m,
            Start = DateTime.UtcNow.AddDays(1),
            End = DateTime.UtcNow.AddDays(15)
        };

        // Unpaid contract still in payment period (should count full amount in predicted)
        var unpaidContract = new Contract
        {
            Id = 4,
            SoftwareId = 1,
            IsPaid = false,
            IsSigned = false,
            ToPay = 18000m,
            Paid = 0m,
            Start = DateTime.UtcNow.AddDays(2),
            End = DateTime.UtcNow.AddDays(20)
        };

        // Expired unpaid contract (should not count in predicted revenue)
        var expiredContract = new Contract
        {
            Id = 5,
            SoftwareId = 1,
            IsPaid = false,
            IsSigned = false,
            ToPay = 10000m,
            Paid = 2000m,
            Start = DateTime.UtcNow.AddDays(-40),
            End = DateTime.UtcNow.AddDays(-35) // Expired
        };

        _fakeSalesRepository.AddTestContract(paidContract1);
        _fakeSalesRepository.AddTestContract(paidContract2);
        _fakeSalesRepository.AddTestContract(partiallyPaidContract);
        _fakeSalesRepository.AddTestContract(unpaidContract);
        _fakeSalesRepository.AddTestContract(expiredContract);

        var request = new RevenueRequestDTO
        {
            SoftwareId = 1,
            Currency = "PLN"
        };

        // Act
        var result = await _salesService.CalculateRevenueAsync(request);

        // Assert
        // Current revenue: 12000 + 15000 = 27000 (only fully paid contracts)
        Assert.Equal(27000m, result.CurrentRevenue);
        
        // Predicted revenue: Current (27000) + Unpaid from active contracts (20000 + 18000) = 65000
        Assert.Equal(65000m, result.PredictedRevenue);
        
        Assert.Equal("Enterprise Software", result.SoftwareName);
        Assert.Equal(5, result.TotalContracts);
        Assert.Equal(2, result.PaidContracts); // Only contracts with IsPaid = true
        Assert.Equal(3, result.UnpaidContracts);
    }

    [Fact]
    public async Task CalculateRevenueAsync_ShouldHandleEmptyDatabase_Gracefully()
    {
        // Arrange
        _fakeSalesRepository.ClearData();

        var request = new RevenueRequestDTO
        {
            SoftwareId = 1,
            Currency = "USD"
        };

        // Act
        var result = await _salesService.CalculateRevenueAsync(request);

        // Assert
        Assert.Equal(0m, result.CurrentRevenue);
        Assert.Equal(0m, result.PredictedRevenue);
        Assert.Null(result.SoftwareName);
        Assert.Equal(0, result.TotalContracts);
        Assert.Equal(0, result.PaidContracts);
        Assert.Equal(0, result.UnpaidContracts);
        Assert.Equal("USD", result.Currency);
    }
}

// Additional test class for edge cases and error scenarios
public class RevenueCalculationEdgeCasesTests
{
    private readonly FakeSalesRepository _fakeSalesRepository;
    private readonly FakeClientRepository _fakeClientRepository;
    private readonly SalesService _salesService;

    public RevenueCalculationEdgeCasesTests()
    {
        _fakeSalesRepository = new FakeSalesRepository();
        _fakeClientRepository = new FakeClientRepository();
        _salesService = new SalesService(_fakeSalesRepository, _fakeClientRepository);
    }

    [Fact]
    public async Task CalculateRevenueAsync_ShouldHandleZeroAmountContracts()
    {
        // Arrange
        var zeroAmountContract = new Contract
        {
            Id = 1,
            SoftwareId = 1,
            IsPaid = true,
            IsSigned = true,
            ToPay = 0m,
            Paid = 0m,
            Start = DateTime.UtcNow.AddDays(-10),
            End = DateTime.UtcNow.AddDays(-5)
        };
        _fakeSalesRepository.AddTestContract(zeroAmountContract);

        var request = new RevenueRequestDTO { Currency = "PLN" };

        // Act
        var result = await _salesService.CalculateRevenueAsync(request);

        // Assert
        Assert.Equal(0m, result.CurrentRevenue);
        Assert.Equal(0m, result.PredictedRevenue);
        Assert.Equal(1, result.TotalContracts);
        Assert.Equal(1, result.PaidContracts);
        Assert.Equal(0, result.UnpaidContracts);
    }

    [Fact]
    public async Task CalculateRevenueAsync_ShouldHandleContractsWithExactExpirationDate()
    {
        // Arrange
        var contractExpiringToday = new Contract
        {
            Id = 1,
            SoftwareId = 1,
            IsPaid = false,
            IsSigned = false,
            ToPay = 1000m,
            Paid = 0m,
            Start = DateTime.UtcNow.AddDays(-10),
            End = DateTime.UtcNow // Expires exactly today
        };
        _fakeSalesRepository.AddTestContract(contractExpiringToday);

        var request = new RevenueRequestDTO { Currency = "PLN" };

        // Act
        var result = await _salesService.CalculateRevenueAsync(request);

        // Assert
        // Contract expiring today should not be included in predicted revenue
        Assert.Equal(0m, result.CurrentRevenue);
        Assert.Equal(0m, result.PredictedRevenue);
        Assert.Equal(1, result.TotalContracts);
        Assert.Equal(0, result.PaidContracts);
        Assert.Equal(1, result.UnpaidContracts);
    }

    [Fact]
    public async Task CalculateRevenueAsync_ShouldHandleLargeAmounts()
    {
        // Arrange
        var largeAmountContract = new Contract
        {
            Id = 1,
            SoftwareId = 1,
            IsPaid = true,
            IsSigned = true,
            ToPay = 999999.99m,
            Paid = 999999.99m,
            Start = DateTime.UtcNow.AddDays(-10),
            End = DateTime.UtcNow.AddDays(-5)
        };
        _fakeSalesRepository.AddTestContract(largeAmountContract);

        var request = new RevenueRequestDTO
        {
            Currency = "USD" // 0.25 exchange rate
        };

        // Act
        var result = await _salesService.CalculateRevenueAsync(request);

        // Assert
        var expectedUSD = Math.Round(999999.99m * 0.25m, 2);
        Assert.Equal(expectedUSD, result.CurrentRevenue);
        Assert.Equal(expectedUSD, result.PredictedRevenue);
    }

    [Fact]
    public async Task CalculateRevenueAsync_ShouldHandlePartiallyPaidContracts_InPredictedRevenue()
    {
        // Arrange
        var partiallyPaidContract = new Contract
        {
            Id = 1,
            SoftwareId = 1,
            IsPaid = false,
            IsSigned = false,
            ToPay = 1000m,
            Paid = 300m, // Partially paid
            Start = DateTime.UtcNow.AddDays(1),
            End = DateTime.UtcNow.AddDays(10)
        };
        _fakeSalesRepository.AddTestContract(partiallyPaidContract);

        var request = new RevenueRequestDTO { Currency = "PLN" };

        // Act
        var result = await _salesService.CalculateRevenueAsync(request);

        // Assert
        Assert.Equal(0m, result.CurrentRevenue); // Not fully paid, so no current revenue
        Assert.Equal(1000m, result.PredictedRevenue); // Full contract amount included in predicted
        Assert.Equal(1, result.TotalContracts);
        Assert.Equal(0, result.PaidContracts);
        Assert.Equal(1, result.UnpaidContracts);
    }
}