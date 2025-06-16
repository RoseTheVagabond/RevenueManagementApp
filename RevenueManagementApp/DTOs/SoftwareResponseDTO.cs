namespace RevenueManagementApp.DTOs;

public class SoftwareResponseDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string CurrentVersion { get; set; } = null!;
    public int CathegoryId { get; set; }
    public decimal Price { get; set; }
    public CategoryResponseDTO Cathegory { get; set; } = null!;
}

public class CategoryResponseDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}