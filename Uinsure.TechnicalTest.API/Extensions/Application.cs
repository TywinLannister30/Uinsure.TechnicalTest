using Uinsure.TechnicalTest.Application.Services.PolicyService;

namespace Uinsure.TechnicalTest.API.Extensions;

public static class Application
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<IPolicyService, PolicyService>();
    }
}
