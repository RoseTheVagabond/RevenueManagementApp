using System;
using System.Collections.Generic;

namespace RevenueManagementApp.Models;

public partial class Individual
{
    public int ClientId { get; set; }

    public string Pesel { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public DateTime? DeletedAt { get; set; }

    public virtual Client Client { get; set; } = null!;
}
