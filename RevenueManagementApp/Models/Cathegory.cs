using System;
using System.Collections.Generic;

namespace RevenueManagementApp.Models;

public partial class Cathegory
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Software> Softwares { get; set; } = new List<Software>();
}
