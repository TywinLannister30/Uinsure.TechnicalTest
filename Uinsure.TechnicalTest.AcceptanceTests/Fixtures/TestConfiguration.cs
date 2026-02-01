using Microsoft.Extensions.Configuration;

namespace Uinsure.TechnicalTest.AcceptanceTests.Fixtures;

public static class TestConfiguration
{
    public static IConfiguration Build()
    {
        return new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
    }
}
