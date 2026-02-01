using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Uinsure.TechnicalTest.Application.Configuration;
using Uinsure.TechnicalTest.Application.Services.PolicyCancellationService.RefundProcessors;

namespace Uinsure.TechnicalTest.Application.Services.PolicyCancellationService.Factories;

public class RefundProcessorFactory(IServiceProvider serviceProvider) : IRefundProcessorFactory
{
    public IRefundProcessor GetRefundProcessor(DateTimeOffset policyStartDate, DateTimeOffset policyCancellationDate)
    {
        var settings = serviceProvider.GetService(typeof(IOptions<PolicySettings>)) as IOptions<PolicySettings>;

        var coolingOffPeriodDays = settings?.Value?.CoolingOffPeriodDays;

        if (coolingOffPeriodDays == null)
            throw new ArgumentNullException("PolicySettings configuration is missing.");

        if (policyCancellationDate <= policyStartDate.AddDays(coolingOffPeriodDays.Value))
            return serviceProvider.GetServices<IRefundProcessor>().OfType<FullRefundProcessor>().Single();

        return serviceProvider.GetServices<IRefundProcessor>().OfType<ProRataRefundProcessor>().Single();
    }
}
