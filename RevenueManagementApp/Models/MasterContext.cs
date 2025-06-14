using System;
using System.Collections.Generic;
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

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Contract> Contracts { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<Individual> Individuals { get; set; }

    public virtual DbSet<Software> Softwares { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=localhost;Database=master;User Id=sa;Password=Strong*Password123;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Cathegory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Cathegory_pk");

            entity.ToTable("Cathegory");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Client_pk");

            entity.ToTable("Client");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.ClientType)
                .HasMaxLength(10)
                .HasColumnName("clientType");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.ClientId).HasName("Company_pk");

            entity.ToTable("Company");

            entity.Property(e => e.ClientId)
                .ValueGeneratedNever()
                .HasColumnName("Client_id");
            entity.Property(e => e.Address)
                .HasMaxLength(200)
                .HasColumnName("address");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Krs)
                .HasMaxLength(10)
                .HasColumnName("KRS");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(14)
                .HasColumnName("phoneNumber");

            entity.HasOne(d => d.Client).WithOne(p => p.Company)
                .HasForeignKey<Company>(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Company_Client");
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Contract_pk");

            entity.ToTable("Contract");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.ClientId).HasColumnName("Client_id");
            entity.Property(e => e.DiscountId).HasColumnName("Discount_id");
            entity.Property(e => e.End)
                .HasColumnType("datetime")
                .HasColumnName("end");
            entity.Property(e => e.IsPaid).HasColumnName("isPaid");
            entity.Property(e => e.IsSigned).HasColumnName("isSigned");
            entity.Property(e => e.Paid)
                .HasColumnType("decimal(8, 2)")
                .HasColumnName("paid");
            entity.Property(e => e.SoftwareDeadline)
                .HasColumnType("datetime")
                .HasColumnName("softwareDeadline");
            entity.Property(e => e.SoftwareId).HasColumnName("Software_id");
            entity.Property(e => e.Start)
                .HasColumnType("datetime")
                .HasColumnName("start");
            entity.Property(e => e.ToPay)
                .HasColumnType("decimal(8, 2)")
                .HasColumnName("toPay");

            entity.HasOne(d => d.Client).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Contract_Client");

            entity.HasOne(d => d.Discount).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.DiscountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Contract_Discount");

            entity.HasOne(d => d.Software).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.SoftwareId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Contract_Software");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Discount_pk");

            entity.ToTable("Discount");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.End)
                .HasColumnType("datetime")
                .HasColumnName("end");
            entity.Property(e => e.Percentage).HasColumnName("percentage");
            entity.Property(e => e.Start)
                .HasColumnType("datetime")
                .HasColumnName("start");
        });

        modelBuilder.Entity<Individual>(entity =>
        {
            entity.HasKey(e => e.ClientId).HasName("Individual_pk");

            entity.ToTable("Individual");

            entity.Property(e => e.ClientId)
                .ValueGeneratedNever()
                .HasColumnName("Client_id");
            entity.Property(e => e.Address)
                .HasMaxLength(200)
                .HasColumnName("address");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deletedAt");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("firstName");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("lastName");
            entity.Property(e => e.Pesel)
                .HasMaxLength(11)
                .HasColumnName("PESEL");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(14)
                .HasColumnName("phoneNumber");

            entity.HasOne(d => d.Client).WithOne(p => p.Individual)
                .HasForeignKey<Individual>(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Individual_Client");
        });

        modelBuilder.Entity<Software>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Software_pk");

            entity.ToTable("Software");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CathegoryId).HasColumnName("Cathegory_id");
            entity.Property(e => e.CurrentVersion)
                .HasMaxLength(5)
                .HasColumnName("currentVersion");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");

            entity.HasOne(d => d.Cathegory).WithMany(p => p.Softwares)
                .HasForeignKey(d => d.CathegoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Software_Cathegory");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
