using Microsoft.EntityFrameworkCore;
using Uinsure.TechnicalTest.Domain.Agregates;

namespace Uinsure.TechnicalTest.Infrastructure.EntityFramework.Context;

public interface IPolicyDbContext
{
    DbSet<Policy> Policies { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
