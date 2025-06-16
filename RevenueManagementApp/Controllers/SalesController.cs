using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RevenueManagementApp.DTOs;
using RevenueManagementApp.Services;

namespace RevenueManagementApp.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    private readonly ISalesService _salesService;

    public SalesController(ISalesService salesService)
    {
        _salesService = salesService;
    }

    [HttpPost("discount")]
    public async Task<ActionResult<decimal>> CreateDiscount([FromBody] DiscountDTO discountDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var discount = await _salesService.CreateDiscountAsync(discountDto);
            return Created(string.Empty, discount);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating the discount.", details = ex.Message });
        }
    }

    [HttpPost("contract")]
    public async Task<IActionResult> CreateContract([FromBody] ContractDTO contractDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var contractId = await _salesService.CreateContractAsync(contractDto);
            return Created(string.Empty, $"Created contract with id={contractId}");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating the contract.", details = ex.Message });
        }
    }

    [HttpDelete("contract/{contractId}")]
    public async Task<IActionResult> DeleteContract(int contractId)
    {
        try
        {
            var result = await _salesService.DeleteContractAsync(contractId);
            if (result)
            {
                return Ok(new { message = "Contract has been successfully deleted." });
            }
            return StatusCode(500, new { message = "Failed to delete contract." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while deleting the contract.", details = ex.Message });
        }
    }

    [HttpPost("contract/payment")]
    public async Task<IActionResult> PayForContract([FromBody] ContractPaymentDTO paymentDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var contract = await _salesService.PayForContractAsync(paymentDto);
            
            var response = new
            {
                message = contract.IsPaid == true ? "Contract has been fully paid and signed." : "Payment processed successfully.",
                remainingAmount = contract.ToPay - contract.Paid
            };
            
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while processing the payment.", details = ex.Message });
        }
    }

    [HttpGet("contracts")]
    public async Task<IActionResult> GetContracts()
    {
        try
        {
            var contracts = await _salesService.GetContracts();
            return Ok(contracts);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving contracts.", details = ex.Message });
        }
    }

    [HttpGet("software")]
    public async Task<IActionResult> GetAllSoftware()
    {
        try
        {
            var software = await _salesService.GetAllSoftwareAsync();
            return Ok(software);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving software.", details = ex.Message });
        }
    }
}