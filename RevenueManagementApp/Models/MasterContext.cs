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
                .ValueGeneratedNever(); // Not auto-generated, manually set
            
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

            entity.HasOne(d => d.Cathegory).WithMany(p => p.Softwares)
                .HasForeignKey(d => d.CathegoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Software_Cathegory");
        });
    }
}