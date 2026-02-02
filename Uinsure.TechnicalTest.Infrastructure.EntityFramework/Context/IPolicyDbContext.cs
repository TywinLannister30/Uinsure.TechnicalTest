using Microsoft.EntityFrameworkCore;
using Uinsure.TechnicalTest.Domain.Aggregates;
using Uinsure.TechnicalTest.Domain.Entities;

namespace Uinsure.TechnicalTest.Infrastructure.EntityFramework.Context;

public interface IPolicyDbContext
{
    DbSet<Policy> Policies { get; set; }
    DbSet<Policyholder> Policyholders { get; set; }
    DbSet<Property> Properties { get; set; }
    DbSet<Payment> Payments { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
