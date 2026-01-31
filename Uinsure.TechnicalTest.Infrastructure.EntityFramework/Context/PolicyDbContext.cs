using Microsoft.EntityFrameworkCore;
using Uinsure.TechnicalTest.Domain.Agregates;

namespace Uinsure.TechnicalTest.Infrastructure.EntityFramework.Context;

public class PolicyDbContext(DbContextOptions options) : DbContext(options), IPolicyDbContext
{
    public DbSet<Policy> Policies { get; set; }
}
