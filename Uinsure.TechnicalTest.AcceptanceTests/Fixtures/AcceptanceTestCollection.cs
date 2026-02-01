namespace Uinsure.TechnicalTest.AcceptanceTests.Fixtures;

[CollectionDefinition("Acceptance tests")]
public sealed class AcceptanceTestCollection :
    ICollectionFixture<HttpClientFixture>,
    ICollectionFixture<DatabaseFixture>
{
}
