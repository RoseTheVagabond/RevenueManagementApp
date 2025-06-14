using System;
using System.Collections.Generic;

namespace RevenueManagementApp.Models;

public partial class Contract
{
    public int Id { get; set; }

    public DateTime Start { get; set; }

    public DateTime? End { get; set; }

    public DateTime SoftwareDeadline { get; set; }

    public bool IsSigned { get; set; }

    public bool IsPaid { get; set; }

    public decimal ToPay { get; set; }

    public decimal Paid { get; set; }

    public int ClientId { get; set; }

    public int SoftwareId { get; set; }

    public int DiscountId { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual Discount Discount { get; set; } = null!;

    public virtual Software Software { get; set; } = null!;
}
