using System.ComponentModel.DataAnnotations;

namespace RevenueManagementApp.Models.auth;

public class LoginDTO
{
    [Required]
    public string Username { get; set; } = string.Empty;
    
    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
}