using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RevenueManagementApp.DTOs;
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
    
    [HttpGet]
    public async Task<IActionResult> GetAllClients()
    {
        try
        {
            var clients = await _service.GetAllClientsAsync();
            return Ok(clients);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);        
        }
    }

    [HttpGet("individuals")]
    public async Task<IActionResult> GetAllIndividuals()
    {
        try
        {
            var individuals = await _service.GetAllIndividualsAsync();
            return Ok(individuals);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);        
        }
    }

    [HttpGet("companies")]
    public async Task<IActionResult> GetAllCompanies()
    {
        try
        {
            var companies = await _service.GetAllCompaniesAsync();
            return Ok(companies);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);        
        }
    }

    [HttpPost("individual")]
    public async Task<IActionResult> AddIndividual([FromBody] CreateIndividualDto createIndividualDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var individual = await _service.AddIndividualAsync(createIndividualDto);
            return Created("", individual);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpPost("company")]
    public async Task<IActionResult> AddCompany([FromBody] CreateCompanyDto createCompanyDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var company = await _service.AddCompanyAsync(createCompanyDto);
            return Created("", company);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPatch("individual")]
    public async Task<IActionResult> UpdateIndividual([FromBody] UpdateIndividualDto updateIndividualDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var individual = await _service.UpdateIndividualAsync(updateIndividualDto);
            return Ok(individual);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);        }
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPatch("company")]
    public async Task<IActionResult> UpdateCompany([FromBody] UpdateCompanyDto updateCompanyDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var company = await _service.UpdateCompanyAsync(updateCompanyDto);
            return Ok(company);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);        }
    }
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("individual")]
    public async Task<IActionResult> DeleteIndividual(string pesel)
    {
        try
        {
            if (string.IsNullOrEmpty(pesel))
            {
                return BadRequest(new { message = "PESEL is required." });
            }

            if (pesel.Length != 11)
            {
                return BadRequest(new { message = "PESEL must be exactly 11 characters long." });
            }

            var result = await _service.DeleteIndividualAsync(pesel);
            if (result)
            {
                return Ok(new { message = "Individual has been successfully marked as deleted." });
            }
            return StatusCode(500, "Individual could not be deleted.");        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);        }
    }
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("company")]
    public async Task<IActionResult> DeleteCompany(string krs)
    {
        return BadRequest(new { message = "Company cannot be deleted from the database" });
    }
}