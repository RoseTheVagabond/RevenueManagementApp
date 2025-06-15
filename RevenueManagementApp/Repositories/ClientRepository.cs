using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RevenueManagementApp.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly IdentityDbContext _context;

    public ClientRepository(IdentityDbContext context)
    {
        _context = context;
    }
}