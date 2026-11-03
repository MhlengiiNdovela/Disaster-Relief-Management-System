using System;
using System.Collections.Generic;
using Service.Models;
using Microsoft.EntityFrameworkCore;

namespace Service.Data;

public partial class GiftDbContext : DbContext
{
    public GiftDbContext()
    {
    }

    public GiftDbContext(DbContextOptions<GiftDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Donation> Donations { get; set; }

    public virtual DbSet<ReliefProject> ReliefProjects { get; set; }

    public virtual DbSet<ReliefRequest> ReliefRequests { get; set; }

    public virtual DbSet<Resource> Resources { get; set; }

    public virtual DbSet<ResourceAllocation> ResourceAllocations { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Volunteer> Volunteers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // DbContext is configured in Program.cs via DI. Do not store connection strings in code.
        // If the context is used outside DI, configure it here using configuration or environment variables.
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var utcDefault = Database.ProviderName?.Contains("Sqlite", StringComparison.OrdinalIgnoreCase) == true
            ? "CURRENT_TIMESTAMP"
            : "(getdate())";

        modelBuilder.Entity<Donation>(entity =>
        {
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Currency).HasMaxLength(3);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.DonationDate).HasDefaultValueSql(utcDefault);
            entity.Property(e => e.DonationType).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Received");

            entity.HasOne(d => d.DonorUser).WithMany(p => p.Donations)
                .HasForeignKey(d => d.DonorUserId)
                .HasConstraintName("FK_Donations_Users");

            entity.HasOne(d => d.Project).WithMany(p => p.Donations)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Donations_ReliefProjects");
        });

        modelBuilder.Entity<ReliefProject>(entity =>
        {
            entity.HasKey(e => e.ProjectId);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql(utcDefault);
            entity.Property(e => e.Location).HasMaxLength(255);
            entity.Property(e => e.ProjectName).HasMaxLength(200);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Planning");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.ReliefProjects)
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReliefProjects_Users");
        });

        modelBuilder.Entity<ReliefRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId);

            entity.Property(e => e.Priority)
                .HasMaxLength(50)
                .HasDefaultValue("Medium");
            entity.Property(e => e.RequestType).HasMaxLength(100);
            entity.Property(e => e.RequestedAt).HasDefaultValueSql(utcDefault);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Project).WithMany(p => p.ReliefRequests)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReliefRequests_ReliefProjects");

            entity.HasOne(d => d.RequestedByUser).WithMany(p => p.ReliefRequests)
                .HasForeignKey(d => d.RequestedByUserId)
                .HasConstraintName("FK_ReliefRequests_Users");
        });

        modelBuilder.Entity<Resource>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql(utcDefault);
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.Property(e => e.Location).HasMaxLength(255);
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ResourceName).HasMaxLength(200);
            entity.Property(e => e.ResourceType).HasMaxLength(100);
            entity.Property(e => e.Unit).HasMaxLength(50);
        });

        modelBuilder.Entity<ResourceAllocation>(entity =>
        {
            entity.HasKey(e => e.AllocationId);

            entity.Property(e => e.AllocationDate).HasDefaultValueSql(utcDefault);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.AllocatedByUser).WithMany(p => p.ResourceAllocations)
                .HasForeignKey(d => d.AllocatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResourceAllocations_Users");

            entity.HasOne(d => d.Project).WithMany(p => p.ResourceAllocations)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResourceAllocations_ReliefProjects");

            entity.HasOne(d => d.Resource).WithMany(p => p.ResourceAllocations)
                .HasForeignKey(d => d.ResourceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResourceAllocations_Resources");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email, "UQ_Users_Email").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql(utcDefault);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(500);
            entity.Property(e => e.Role).HasMaxLength(50);
        });

        modelBuilder.Entity<Volunteer>(entity =>
        {
            entity.HasIndex(e => e.UserId, "UQ_Volunteers_UserId").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Availability).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql(utcDefault);
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.Property(e => e.PhoneNumber).HasMaxLength(30);
            entity.Property(e => e.Skills).HasMaxLength(500);

            entity.HasOne(d => d.User).WithOne(p => p.Volunteer)
                .HasForeignKey<Volunteer>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Volunteers_Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
