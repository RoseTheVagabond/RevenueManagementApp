using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RevenueManagementApp.Models.auth;

namespace RevenueManagementApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] LoginDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
        {
            return BadRequest("Username and password are required");
        }
        
        var existingUser = await _userManager.FindByNameAsync(dto.Username);
        if (existingUser != null)
        {
            return BadRequest("Username already exists");
        }

        var user = new IdentityUser
        {
            UserName = dto.Username,
            Email = null,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "Employee");
            
            return Ok(new { 
                Message = "User registered successfully",
                Username = user.UserName,
                Role = "Employee"
            });
        }

        return BadRequest(result.Errors.Select(e => e.Description));
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
        {
            return BadRequest("Username and password are required");
        }

        var user = await _userManager.FindByNameAsync(dto.Username);
        if (user == null)
        {
            return Unauthorized("Invalid username or password");
        }

        var result = await _signInManager.PasswordSignInAsync(
            user, 
            dto.Password, 
            isPersistent: true,
            lockoutOnFailure: true
        );
        
        if (result.Succeeded)
        {
            var roles = await _userManager.GetRolesAsync(user);
            
            return Ok(new
            {
                Message = "Login successful",
                Username = user.UserName,
                Roles = roles,
                IsAdmin = roles.Contains("Admin"),
                IsEmployee = roles.Contains("Employee")
            });
        }
        
        if (result.IsLockedOut)
        {
            return BadRequest("Account is temporarily locked due to too many failed attempts");
        }
        
        return Unauthorized("Invalid username or password");
    }
    
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok(new { Message = "Logged out successfully" });
    }
}