using RevenueManagementApp.DTOs;
using RevenueManagementApp.Models;
using RevenueManagementApp.Repositories;

namespace RevenueManagementApp.Services;

public class SalesService : ISalesService
{
    private readonly ISalesRepository _salesRepository;
    private readonly IClientRepository _clientRepository;
    private const decimal AdditionalSupportCost = 1000m;
    
    public SalesService(ISalesRepository salesRepository, IClientRepository clientRepository)
    {
        _salesRepository = salesRepository;
        _clientRepository = clientRepository;
    }
    
    public async Task<Discount> CreateDiscountAsync(DiscountDTO discountDto)
    {

        if (discountDto.Start >= discountDto.End)
        {
            throw new ArgumentException("Start date must be before end date.");
        }

        if (discountDto.Start < DateTime.UtcNow.Date)
        {
            throw new ArgumentException("Start date cannot be in the past.");
        }

        var discount = new Discount
        {
            Percentage = discountDto.Percentage,
            Start = discountDto.Start,
            End = discountDto.End
        };

        var createdDiscount = await _salesRepository.CreateDiscountAsync(discount);
        return createdDiscount;
    }
    
    public async Task<List<Discount>> GetActiveDiscountsAsync()
    {
        return await _salesRepository.GetActiveDiscountsAsync();
    }
    
    public async Task<int> CreateContractAsync(ContractDTO contractDto)
    {
        if (string.IsNullOrEmpty(contractDto.IndividualPesel) && string.IsNullOrEmpty(contractDto.CompanyKrs))
        {
            throw new ArgumentException("Either Individual PESEL or Company KRS must be provided.");
        }

        if (!string.IsNullOrEmpty(contractDto.IndividualPesel) && !string.IsNullOrEmpty(contractDto.CompanyKrs))
        {
            throw new ArgumentException("Cannot specify both Individual PESEL and Company KRS.");
        }

        if (!string.IsNullOrEmpty(contractDto.IndividualPesel))
        {
            if (!await _clientRepository.IndividualExistsAsync(contractDto.IndividualPesel))
            {
                throw new ArgumentException("Specified client does not exist.");
            }
        } else if (!string.IsNullOrEmpty(contractDto.CompanyKrs))
        {
            if (!await _clientRepository.CompanyExistsAsync(contractDto.CompanyKrs))
            {
                throw new ArgumentException("Specified client does not exist.");
            }
        }
        
        var software = await _salesRepository.GetSoftwareByIdAsync(contractDto.SoftwareId);
        if (software == null)
        {
            throw new KeyNotFoundException("Specified software does not exist.");
        }
        
        if (contractDto.Start >= contractDto.End)
        {
            throw new ArgumentException("Start date must be before end date.");
        }

        var timespan = contractDto.End - contractDto.Start;
        if (timespan.TotalDays < 3 || timespan.TotalDays > 30)
        {
            throw new ArgumentException("Contract duration must be between 3 and 30 days.");
        }
        
        if (await _salesRepository.HasActiveSubscriptionForSoftwareAsync(
            contractDto.IndividualPesel, contractDto.CompanyKrs, contractDto.SoftwareId))
        {
            throw new InvalidOperationException("Client already has an active subscription for this software.");
        }
        
        var activeDiscounts = await _salesRepository.GetActiveDiscountsAsync();
        var discount = activeDiscounts.OrderByDescending(d => d.Percentage).FirstOrDefault();
        
        var softwareDeadline = contractDto.Start.AddYears(1 + contractDto.AdditionalSupportYears);
        
        decimal totalPrice = software.Price;
        
        totalPrice += contractDto.AdditionalSupportYears * AdditionalSupportCost;

        if (discount != null)
        {
            var discountAmount = totalPrice * (discount.Percentage / 100m);
            totalPrice -= discountAmount;
        }

        var contract = new Contract
        {
            IndividualPesel = contractDto.IndividualPesel,
            CompanyKrs = contractDto.CompanyKrs,
            SoftwareId = contractDto.SoftwareId,
            DiscountId = discount?.Id,
            Start = contractDto.Start,
            End = contractDto.End,
            SoftwareDeadline = softwareDeadline,
            IsSigned = false,
            IsPaid = false,
            ToPay = totalPrice,
            Paid = 0
        };

        var createdContractId = await _salesRepository.CreateContractAsync(contract);

        return createdContractId;
    }

    public async Task<bool> DeleteContractAsync(int contractId)
    {
        var contract = await _salesRepository.GetContractByIdAsync(contractId);
        if (contract == null)
        {
            throw new KeyNotFoundException("Contract not found.");
        }
        
        if (contract.IsSigned == true)
        {
            throw new InvalidOperationException("Cannot delete a signed contract.");
        }

        return await _salesRepository.DeleteContractAsync(contractId);
    }

    public async Task<Contract> PayForContractAsync(ContractPaymentDTO paymentDto)
    {
        var contract = await _salesRepository.GetContractByIdAsync(paymentDto.ContractId);
        if (contract == null)
        {
            throw new KeyNotFoundException("Contract not found.");
        }
        
        if (DateTime.Now > contract.End)
        {

            if (await _salesRepository.DeleteContractAsync(contract.Id))
            {
                throw new InvalidOperationException($"Payment period for this contract has expired. Refund of {contract.Paid}PLN will be issued and the contract has been deleted.");
            }
            throw new InvalidOperationException($"Payment period for this contract has expired. Refund of {contract.Paid}PLN will be issued, but the contract couldn't be deleted.");

            
        }
        
        var remainingAmount = contract.ToPay - contract.Paid;
        
        if (remainingAmount <= 0)
        {
            throw new ArgumentException("Contract is already fully paid.");
        }
        
        if (paymentDto.PaymentAmount > remainingAmount)
        {
            throw new ArgumentException($"Payment amount exceeds remaining balance. to pay: ${remainingAmount}.");
        }
            
        contract.Paid += paymentDto.PaymentAmount;
        
        if (contract.Paid >= contract.ToPay)
        {
            contract.IsPaid = true;
            contract.IsSigned = true;
        }

        await _salesRepository.UpdateContractAsync(contract);
        return contract;
    }

    public async Task<List<ContractDTO>> GetContracts()
    {
        var contracts = await _salesRepository.GetContracts();
        var contractsDTOs = new List<ContractDTO>();
    
        foreach (var contract in contracts)
        {
            var conDTO = new ContractDTO
            {
                IndividualPesel = contract.IndividualPesel,
                CompanyKrs = contract.CompanyKrs,
                SoftwareId = contract.SoftwareId,
                Start = contract.Start,
                End = contract.End,
                AdditionalSupportYears = (contract.SoftwareDeadline.Year - contract.Start.Year) - 1
            };
        
            contractsDTOs.Add(conDTO);
        }
    
        return contractsDTOs;
    }
    
    public async Task<List<SoftwareResponseDTO>> GetAllSoftwareAsync()
    {
        var software = await _salesRepository.GetAllSoftwareAsync();
        return software.Select(s => new SoftwareResponseDTO
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description,
            CurrentVersion = s.CurrentVersion,
            CathegoryId = s.CathegoryId,
            Price = s.Price,
            Cathegory = new CategoryResponseDTO
            {
                Id = s.Cathegory.Id,
                Name = s.Cathegory.Name
            }
        }).ToList();
    }
}