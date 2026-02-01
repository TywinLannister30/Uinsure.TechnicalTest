using Uinsure.TechnicalTest.Domain.Repository;
using Uinsure.TechnicalTest.Infrastructure.EntityFramework.Repository;

namespace Uinsure.TechnicalTest.API.Extensions;

public static class Infrastructure
{
    public static void AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IPolicyRepository, PolicyRepository>();
    }
}
