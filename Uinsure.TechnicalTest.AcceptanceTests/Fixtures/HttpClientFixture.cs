using RestSharp;

namespace Uinsure.TechnicalTest.AcceptanceTests.Fixtures;

public sealed class HttpClientFixture : IDisposable
{
    public RestClient Client { get; }
    public Uri BaseUri { get; }

    public HttpClientFixture()
    {
        var configuration = TestConfiguration.Build();
        var baseUrl = configuration["ApiBaseUrl"] ?? "http://localhost:5019";
        BaseUri = new Uri(baseUrl);

        Client = new RestClient(new RestClientOptions
        {
            BaseUrl = BaseUri,
            ThrowOnAnyError = false
        });
    }

    public void Dispose()
    {
        Client.Dispose();
    }
}
