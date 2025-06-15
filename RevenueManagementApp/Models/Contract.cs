using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RevenueManagementApp.Models;

public partial class Contract
{
    public int Id { get; set; }
    
    [StringLength(11)]
    public string? IndividualPesel { get; set; }
    
    [StringLength(10)]
    public string? CompanyKrs { get; set; }

    public int? SoftwareId { get; set; }
    public int? DiscountId { get; set; }

    public DateTime? Start { get; set; }
    public DateTime? End { get; set; }
    public DateTime? SoftwareDeadline { get; set; }

    public bool? IsSigned { get; set; }
    public bool? IsPaid { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? ToPay { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? Paid { get; set; }
    
    public virtual Individual? Individual { get; set; }
    public virtual Company? Company { get; set; }
    public virtual Software? Software { get; set; }
    public virtual Discount? Discount { get; set; }
}