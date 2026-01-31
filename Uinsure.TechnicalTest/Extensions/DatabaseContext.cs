using Uinsure.TechnicalTest.Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace Uinsure.TechnicalTest.API.Extensions;

public static class DatabaseContext
{
    public static void AddDatabaseContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<PolicyDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped<IPolicyDbContext>(provider => provider.GetRequiredService<PolicyDbContext>());
    }

}
