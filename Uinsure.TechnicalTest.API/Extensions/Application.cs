using Uinsure.TechnicalTest.Application.Services.PolicyCancellation;
using Uinsure.TechnicalTest.Application.Services.PolicyCancellation.Factories;
using Uinsure.TechnicalTest.Application.Services.PolicyCancellation.RefundProcessors;
using Uinsure.TechnicalTest.Application.Services.PolicyClaim;
using Uinsure.TechnicalTest.Application.Services.PolicyCreation;
using Uinsure.TechnicalTest.Application.Services.PolicyRenewal;
using Uinsure.TechnicalTest.Application.Services.PolicyRetrieval;

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
