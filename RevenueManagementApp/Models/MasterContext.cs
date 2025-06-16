using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RevenueManagementApp.Models;

public partial class MasterContext : IdentityDbContext<IdentityUser>
{
    public MasterContext()
    {
    }

    public MasterContext(DbContextOptions<MasterContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cathegory> Cathegories { get; set; }
    public virtual DbSet<Company> Companies { get; set; }
    public virtual DbSet<Contract> Contracts { get; set; }
    public virtual DbSet<Discount> Discounts { get; set; }
    public virtual DbSet<Individual> Individuals { get; set; }
    public virtual DbSet<Software> Softwares { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=localhost;Database=master;User Id=sa;Password=Strong*Password123;TrustServerCertificate=True;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cathegory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Cathegory_pk");
            entity.ToTable("Cathegory");
            entity.Property(e => e.Id).ValueGeneratedNever().HasColumnName("id");
            entity.Property(e => e.Name).HasMaxLength(20).HasColumnName("name");
        });
        
        modelBuilder.Entity<Individual>(entity =>
        {
            entity.HasKey(e => e.Pesel).HasName("Individual_pk");
            entity.ToTable("Individual");
            
            entity.Property(e => e.Pesel)
                .HasMaxLength(11)
                .HasColumnName("PESEL")
                .ValueGeneratedNever(); // Not auto-generated, manually set
            
            entity.Property(e => e.Address).HasMaxLength(200).HasColumnName("address");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime").HasColumnName("deletedAt");
            entity.Property(e => e.Email).HasMaxLength(100).HasColumnName("email");
            entity.Property(e => e.FirstName).HasMaxLength(50).HasColumnName("firstName");
            entity.Property(e => e.LastName).HasMaxLength(50).HasColumnName("lastName");
            entity.Property(e => e.PhoneNumber).HasMaxLength(14).HasColumnName("phoneNumber");
            entity.HasIndex(e => e.Pesel).IsUnique();
        });
        
        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.Krs).HasName("Company_pk");
            entity.ToTable("Company");
            
            entity.Property(e => e.Krs)
                .HasMaxLength(10)
                .HasColumnName("KRS")
                .ValueGeneratedNever();
            
            entity.Property(e => e.Address).HasMaxLength(200).HasColumnName("address");
            entity.Property(e => e.Email).HasMaxLength(100).HasColumnName("email");
            entity.Property(e => e.Name).HasMaxLength(100).HasColumnName("name");
            entity.Property(e => e.PhoneNumber).HasMaxLength(14).HasColumnName("phoneNumber");
            entity.HasIndex(e => e.Krs).IsUnique();
        });
        
        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Contract_pk");
            entity.ToTable("Contract");
            
            entity.Property(e => e.Id).ValueGeneratedNever().HasColumnName("id");
            entity.Property(e => e.IndividualPesel).HasMaxLength(11).HasColumnName("Individual_PESEL");
            entity.Property(e => e.CompanyKrs).HasMaxLength(10).HasColumnName("Company_KRS");
            entity.Property(e => e.DiscountId).HasColumnName("Discount_id");
            entity.Property(e => e.End).HasColumnType("datetime").HasColumnName("end");
            entity.Property(e => e.IsPaid).HasColumnName("isPaid");
            entity.Property(e => e.IsSigned).HasColumnName("isSigned");
            entity.Property(e => e.Paid).HasColumnType("decimal(8, 2)").HasColumnName("paid");
            entity.Property(e => e.SoftwareDeadline).HasColumnType("datetime").HasColumnName("softwareDeadline");
            entity.Property(e => e.SoftwareId).HasColumnName("Software_id");
            entity.Property(e => e.Start).HasColumnType("datetime").HasColumnName("start");
            entity.Property(e => e.ToPay).HasColumnType("decimal(8, 2)").HasColumnName("toPay");
            
            entity.HasOne(d => d.Individual)
                .WithMany(p => p.Contracts)
                .HasForeignKey(d => d.IndividualPesel)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Contract_Individual");
            
            entity.HasOne(d => d.Company)
                .WithMany(p => p.Contracts)
                .HasForeignKey(d => d.CompanyKrs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Contract_Company");
            
            entity.HasOne(d => d.Discount)
                .WithMany(p => p.Contracts)
                .HasForeignKey(d => d.DiscountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Contract_Discount");
            
            entity.HasOne(d => d.Software)
                .WithMany(p => p.Contracts)
                .HasForeignKey(d => d.SoftwareId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Contract_Software");
            
            entity.HasCheckConstraint("CK_Contract_ClientType", 
                "([Individual_PESEL] IS NOT NULL AND [Company_KRS] IS NULL) OR ([Individual_PESEL] IS NULL AND [Company_KRS] IS NOT NULL)");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Discount_pk");
            entity.ToTable("Discount");
            entity.Property(e => e.Id).ValueGeneratedNever().HasColumnName("id");
            entity.Property(e => e.End).HasColumnType("datetime").HasColumnName("end");
            entity.Property(e => e.Percentage).HasColumnName("percentage");
            entity.Property(e => e.Start).HasColumnType("datetime").HasColumnName("start");
        });

        modelBuilder.Entity<Software>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Software_pk");
            entity.ToTable("Software");
            entity.Property(e => e.Id).ValueGeneratedNever().HasColumnName("id");
            entity.Property(e => e.CathegoryId).HasColumnName("Cathegory_id");
            entity.Property(e => e.CurrentVersion).HasMaxLength(5).HasColumnName("currentVersion");
            entity.Property(e => e.Description).HasMaxLength(200).HasColumnName("description");
            entity.Property(e => e.Name).HasMaxLength(50).HasColumnName("name");
            entity.Property(e => e.Price).HasColumnType("decimal(8, 2)").HasColumnName("price");

            entity.HasOne(d => d.Cathegory).WithMany(p => p.Softwares)
                .HasForeignKey(d => d.CathegoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Software_Cathegory");
        });
        
        modelBuilder.Entity<Cathegory>().HasData(
            new Cathegory { Id = 1, Name = "Business Software" },
            new Cathegory { Id = 2, Name = "Gaming" },
            new Cathegory { Id = 3, Name = "Design" }
        );

        modelBuilder.Entity<Software>().HasData(
            new Software { Id = 1, Name = "Office Suite Pro", Description = "Complete office productivity suite", CurrentVersion = "2024", CathegoryId = 1, Price = 4999.99m },
            new Software { Id = 2, Name = "Game Engine X", Description = "Advanced 3D game development engine", CurrentVersion = "5.1.2", CathegoryId = 2, Price = 7999.99m },
            new Software { Id = 3, Name = "Design Studio", Description = "Professional graphic design software", CurrentVersion = "12.3", CathegoryId = 3, Price = 2999.99m },
            new Software { Id = 4, Name = "Code Builder", Description = "Integrated development environment", CurrentVersion = "4.8.1", CathegoryId = 1, Price = 3499.99m },
            new Software { Id = 6, Name = "DesignMaster", Description = "Design templates library", CurrentVersion = "3.5.1", CathegoryId = 3, Price = 1999.99m },
            new Software { Id = 7, Name = "BusinessPlatform", Description = "Online management system", CurrentVersion = "2.1.4", CathegoryId = 1, Price = 5999.99m },
            new Software { Id = 8, Name = "MediaPlayer Pro", Description = "Advanced multimedia player", CurrentVersion = "6.0.2", CathegoryId = 2, Price = 1499.99m }
        );
    }
}