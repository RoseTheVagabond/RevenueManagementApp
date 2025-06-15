using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace RevenueManagementApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SetupController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public SetupController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpPost("create-admin")]
    public async Task<IActionResult> CreateAdmin()
    {
        if (await _roleManager.RoleExistsAsync("Admin"))
        {
            var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
            if (adminUsers.Any())
            {
                return BadRequest("Admin already exists. This endpoint is disabled.");
            }
        }
        
        if (!await _roleManager.RoleExistsAsync("Employee"))
        {
            await _roleManager.CreateAsync(new IdentityRole("Employee"));
        }
        
        if (!await _roleManager.RoleExistsAsync("Admin"))
        {
            await _roleManager.CreateAsync(new IdentityRole("Admin"));
        }
        
        var admin = new IdentityUser
        {
            UserName = "admin",
            Email = null,
            EmailConfirmed = true 
        };

        var result = await _userManager.CreateAsync(admin, "Admin123");
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(admin, "Admin");
            return Ok(new 
            { 
                Message = "First admin created successfully",
                Username = "admin",
                Password = "Admin123"
            });
        }

        return BadRequest(result.Errors.Select(e => e.Description));
    }
}
