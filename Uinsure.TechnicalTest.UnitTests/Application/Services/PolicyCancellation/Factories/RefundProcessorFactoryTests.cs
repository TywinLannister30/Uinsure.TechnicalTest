using Microsoft.Extensions.Options;
using Moq;
using Uinsure.TechnicalTest.Application.Configuration;
using Uinsure.TechnicalTest.Application.Services.PolicyCancellation.Factories;
using Uinsure.TechnicalTest.Application.Services.PolicyCancellation.RefundProcessors;

namespace Uinsure.TechnicalTest.UnitTests.Application.Services.PolicyCancellation.Factories;

public class RefundProcessorFactoryTests
{
    private Mock<IServiceProvider> _mockServiceProvider = new();

    private RefundProcessorFactory CreateSut() => new(_mockServiceProvider.Object);

    public RefundProcessorFactoryTests()
    {
        var options = Options.Create(new PolicySettings { CoolingOffPeriodDays = 14 });

        _mockServiceProvider
            .Setup(sp => sp.GetService(typeof(IOptions<PolicySettings>)))
            .Returns(options);

        _mockServiceProvider
                .Setup(sp => sp.GetService(typeof(IEnumerable<IRefundProcessor>)))
                .Returns(new IRefundProcessor[] { new FullRefundProcessor(), new ProRataRefundProcessor() });
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(13)]
    [InlineData(14)]
    public void GetRefundProcessor_WhenCancellationWithinCoolingOff_ReturnsFullRefundProcessor(int numDays)
    {
        var start = new DateTimeOffset(2026, 01, 01, 0, 0, 0, TimeSpan.Zero);
        var cancellation = start.AddDays(numDays);

        var result = CreateSut().GetRefundProcessor(start, cancellation);

        Assert.IsType<FullRefundProcessor>(result);
    }

    [Theory]
    [InlineData(15)]
    [InlineData(16)]
    [InlineData(20)]
    public void GetRefundProcessor_WhenCancellationOutsideOfCoolingOff_ReturnsProRataRefundProcessor(int numDays)
    {
        var start = new DateTimeOffset(2026, 01, 01, 0, 0, 0, TimeSpan.Zero);
        var cancellation = start.AddDays(numDays);

        var result = CreateSut().GetRefundProcessor(start, cancellation);

        Assert.IsType<ProRataRefundProcessor>(result);
    }
}
