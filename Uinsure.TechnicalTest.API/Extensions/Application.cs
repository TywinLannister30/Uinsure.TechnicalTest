using Uinsure.TechnicalTest.Application.Services.PolicyCancellationService;
using Uinsure.TechnicalTest.Application.Services.PolicyCancellationService.Factories;
using Uinsure.TechnicalTest.Application.Services.PolicyCancellationService.RefundProcessors;
using Uinsure.TechnicalTest.Application.Services.PolicyClaimService;
using Uinsure.TechnicalTest.Application.Services.PolicyCreationService;
using Uinsure.TechnicalTest.Application.Services.PolicyRenewalService;
using Uinsure.TechnicalTest.Application.Services.PolicyRetrievalService;

namespace Uinsure.TechnicalTest.API.Extensions;

public static class Application
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<IPolicyCancellationService, PolicyCancellationService>();
        services.AddTransient<IPolicyClaimService, PolicyClaimService>();
        services.AddTransient<IPolicyCreationService, PolicyCreationService>();
        services.AddTransient<IPolicyRenewalService, PolicyRenewalService>();
        services.AddTransient<IPolicyRetrievalService, PolicyRetrievalService>();

        services.AddTransient<IRefundProcessorFactory, RefundProcessorFactory>();
        services.AddTransient<IRefundProcessor, FullRefundProcessor>();
        services.AddTransient<IRefundProcessor, ProRataRefundProcessor>();
    }
}
