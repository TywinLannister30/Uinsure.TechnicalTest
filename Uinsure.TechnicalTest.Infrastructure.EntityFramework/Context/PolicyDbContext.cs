using Microsoft.EntityFrameworkCore;
using Uinsure.TechnicalTest.Domain.Agregates;
using Uinsure.TechnicalTest.Domain.Entities;

namespace Uinsure.TechnicalTest.Infrastructure.EntityFramework.Context;

public class PolicyDbContext(DbContextOptions options) : DbContext(options), IPolicyDbContext
{
    public DbSet<Policy> Policies { get; set; }
    public DbSet<Policyholder> Policyholders { get; set; }
    public DbSet<Property> Properties { get; set; }
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Policy>(entity =>
        {
            entity.ToTable("Policies");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.StartDate).HasColumnType("datetimeoffset");
            entity.Property(e => e.EndDate).HasColumnType("datetimeoffset");
            entity.Property(e => e.State);
            entity.Property(e => e.InsuranceType);
            entity.Property(e => e.Amount);
            entity.Property(e => e.HasClaims);
            entity.Property(e => e.AutoRenew);
            entity.Property(e => e.CreatedDate).HasColumnType("datetimeoffset");
            entity.Navigation(e => e.Policyholders).AutoInclude();
            entity.Navigation(e => e.Payments).AutoInclude();
            entity.Navigation(e => e.Property).AutoInclude();
        });

        modelBuilder.Entity<Policyholder>(entity =>
        {
            entity.ToTable("Policyholders");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName);
            entity.Property(e => e.LastName);
            entity.Property(e => e.DateOfBirth).HasColumnType("datetimeoffset");
            entity.Property(e => e.CreatedDate).HasColumnType("datetimeoffset");
            entity.Property(e => e.PolicyId);
            entity.HasOne<Policy>().WithMany(p => p.Policyholders).HasForeignKey("PolicyId").HasPrincipalKey(x => x.Id).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Property>(entity =>
        {
            entity.ToTable("Properties");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AddressLine1);
            entity.Property(e => e.AddressLine2);
            entity.Property(e => e.AddressLine3);
            entity.Property(e => e.Postcode);
            entity.Property(e => e.CreatedDate).HasColumnType("datetimeoffset");
            entity.Property(e => e.PolicyId);
            entity.HasOne<Policy>().WithOne(p => p.Property).HasForeignKey<Property>(p => p.PolicyId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("Payments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PaymentReference);
            entity.Property(e => e.Type);
            entity.Property(e => e.CreatedDate).HasColumnType("datetimeoffset");
            entity.Property(e => e.TransactionType);
            entity.Property(e => e.PolicyId);
            entity.HasOne<Policy>().WithMany(p => p.Payments).HasForeignKey("PolicyId").HasPrincipalKey(x => x.Id).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
