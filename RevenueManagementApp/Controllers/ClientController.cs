using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RevenueManagementApp.Models;
using RevenueManagementApp.Services;

namespace RevenueManagementApp.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ClientController : ControllerBase
{
    private readonly IClientService _service;

    public ClientController(IClientService service)
    {
        _service = service;
    }

    [HttpPost("individual")]
    public async Task<IActionResult> AddIndividual([FromBody] Individual individual)
    {
        
    }
    
    [HttpPost("company")]
    public async Task<IActionResult> AddCompany([FromBody] Company company)
    {
        
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPatch("individual")]
    public async Task<IActionResult> UpdateIndividual([FromBody] Individual individual)
    {
        
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPatch("company")]
    public async Task<IActionResult> UpdateCompany([FromBody] Company company)
    {
        
    }
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("individual")]
    public async Task<IActionResult> DeleteIndividual([FromBody] string pesel)
    {
        
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost("company")]
    public async Task<IActionResult> DeleteCompany([FromBody] string krs)
    {
        return BadRequest("Company cannot be deleted from the database");
    }
}