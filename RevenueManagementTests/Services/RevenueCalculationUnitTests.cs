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
    public async Task CalculateRevenueAsync_ShouldIncludeUnpaidContractsInPaymentPeriod_InPredictedRevenue()
    {
        var paidContract = new Contract
        {
            Id = 1,
            SoftwareId = 1,
            IsPaid = true,
            IsSigned = true,
            ToPay = 1000m,
            Paid = 1000m,
            Start = DateTime.Now.AddDays(-10),
            End = DateTime.Now.AddDays(-5)
        };

        var unpaidContractInPaymentPeriod = new Contract
        {
            Id = 2,
            SoftwareId = 1,
            IsPaid = false,
            IsSigned = false,
            ToPay = 2000m,
            Paid = 0m,
            Start = DateTime.Now.AddDays(1),
            End = DateTime.Now.AddDays(10) // Still in payment period
        };

        var unpaidContractExpired = new Contract
        {
            Id = 3,
            SoftwareId = 1,
            IsPaid = false,
            IsSigned = false,
            ToPay = 1500m,
            Paid = 0m,
            Start = DateTime.Now.AddDays(-20),
            End = DateTime.Now.AddDays(-10)
        };

        _fakeSalesRepository.AddTestContract(paidContract);
        _fakeSalesRepository.AddTestContract(unpaidContractInPaymentPeriod);
        _fakeSalesRepository.AddTestContract(unpaidContractExpired);

        var request = new RevenueRequestDTO { Currency = "PLN" };
        
        var result = await _salesService.CalculateRevenueAsync(request);
        
        Assert.Equal(1000m, result.CurrentRevenue);
        Assert.Equal(3000m, result.PredictedRevenue);
    }

    [Fact]
    public async Task CalculateRevenueAsync_ShouldFilterBySpecificSoftware_WhenSoftwareIdProvided()
    {
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
            Start = DateTime.Now.AddDays(-10),
            End = DateTime.Now.AddDays(-5)
        };

        var contractSoftware2 = new Contract
        {
            Id = 2,
            SoftwareId = 2,
            IsPaid = true,
            IsSigned = true,
            ToPay = 2000m,
            Paid = 2000m,
            Start = DateTime.Now.AddDays(-8),
            End = DateTime.Now.AddDays(-3)
        };

        var anotherContractSoftware1 = new Contract
        {
            Id = 3,
            SoftwareId = 1,
            IsPaid = false,
            IsSigned = false,
            ToPay = 1500m,
            Paid = 0m,
            Start = DateTime.Now.AddDays(1),
            End = DateTime.Now.AddDays(10)
        };

        _fakeSalesRepository.AddTestContract(contractSoftware1);
        _fakeSalesRepository.AddTestContract(contractSoftware2);
        _fakeSalesRepository.AddTestContract(anotherContractSoftware1);

        var request = new RevenueRequestDTO
        {
            SoftwareId = 1,
            Currency = "PLN"
        };
        
        var result = await _salesService.CalculateRevenueAsync(request);
        
        Assert.Equal(1000m, result.CurrentRevenue);
        Assert.Equal(2500m, result.PredictedRevenue);
        Assert.Equal("Software One", result.SoftwareName);
        Assert.Equal(2, result.TotalContracts);
        Assert.Equal(1, result.PaidContracts);
        Assert.Equal(1, result.UnpaidContracts);
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
    public async Task GetExchangeRateAsync_ShouldThrowException_WhenUnsupportedCurrency()
    {
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _salesService.GetExchangeRateAsync("INVALID"));
        
        Assert.Contains("Currency INVALID not supported", exception.Message);
    }
}