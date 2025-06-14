using System;
using System.Collections.Generic;

namespace RevenueManagementApp.Models;

public partial class Company
{
    public int ClientId { get; set; }

    public string Krs { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public virtual Client Client { get; set; } = null!;
}
