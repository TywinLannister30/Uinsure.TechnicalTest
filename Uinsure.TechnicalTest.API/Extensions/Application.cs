using Uinsure.TechnicalTest.Application.Services.PolicyCancellationService;
using Uinsure.TechnicalTest.Application.Services.PolicyCancellationService.Factories;
using Uinsure.TechnicalTest.Application.Services.PolicyCancellationService.RefundProcessors;
using Uinsure.TechnicalTest.Application.Services.PolicyService;

namespace Uinsure.TechnicalTest.API.Extensions;

public static class Application
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<IPolicyService, PolicyService>();
        services.AddTransient<IPolicyCancellationService, PolicyCancellationService>();

        services.AddTransient<IRefundProcessorFactory, RefundProcessorFactory>();
        services.AddTransient<IRefundProcessor, FullRefundProcessor>();
        services.AddTransient<IRefundProcessor, ProRataRefundProcessor>();
    }
}
