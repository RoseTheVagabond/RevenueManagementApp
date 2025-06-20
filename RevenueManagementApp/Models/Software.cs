﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RevenueManagementApp.Models;

public partial class Software
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string CurrentVersion { get; set; } = null!;

    public int CathegoryId { get; set; }

    [Range(0, 999999)]
    public decimal Price { get; set; }

    public virtual Cathegory Cathegory { get; set; } = null!;

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}