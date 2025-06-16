using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RevenueManagementApp.DTOs;
using RevenueManagementApp.Services;

namespace RevenueManagementApp.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class RevenueController : ControllerBase
{
    private readonly ISalesService _salesService;

    public RevenueController(ISalesService salesService)
    {
        _salesService = salesService;
    }

    [HttpGet]
    public async Task<IActionResult> CalculateRevenue([FromQuery] RevenueRequestDTO request)
    {
        try
        {
            var revenue = await _salesService.CalculateRevenueAsync(request);
            return Ok(revenue);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while calculating revenue.", details = ex.Message });
        }
    }
}