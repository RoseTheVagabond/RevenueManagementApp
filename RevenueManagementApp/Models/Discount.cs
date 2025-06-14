using System;
using System.Collections.Generic;

namespace RevenueManagementApp.Models;

public partial class Discount
{
    public int Id { get; set; }

    public int Percentage { get; set; }

    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}
