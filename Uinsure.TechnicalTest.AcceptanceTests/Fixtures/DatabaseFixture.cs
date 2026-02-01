using Microsoft.Data.SqlClient;

namespace Uinsure.TechnicalTest.AcceptanceTests.Fixtures;

public sealed class DatabaseFixture
{
    public string ConnectionString { get; }

    public DatabaseFixture()
    {
        var configuration = TestConfiguration.Build();
        ConnectionString = configuration.GetSection("ConnectionStrings")["PolicyContext"]
            ?? "Data Source=127.0.0.1,1433;Initial Catalog=UinsureTechnicalTest;MultipleActiveResultSets=True;User id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True";
    }

    public async Task<int> ExecuteAsync(string sql, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        return await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<T?> QueryScalarAsync<T>(string sql, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        var result = await command.ExecuteScalarAsync(cancellationToken);
        if (result is null || result is DBNull)
        {
            return default;
        }

        return (T)result;
    }

    public async Task ClearDataAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
            DELETE FROM [dbo].[Payments];
            DELETE FROM [dbo].[Properties];
            DELETE FROM [dbo].[Policyholders];
            DELETE FROM [dbo].[Policies];
            """;

        await ExecuteAsync(sql, cancellationToken);
    }
}
