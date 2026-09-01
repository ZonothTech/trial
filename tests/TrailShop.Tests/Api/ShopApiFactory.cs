using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;

namespace TrailShop.Tests.Api;

/// <summary>
/// Hosts the API in memory for integration tests, backed by a throwaway SQLite file.
/// </summary>
public class ShopApiFactory : WebApplicationFactory<Program>
{
    public const string DemoUserHeader = "X-Demo-User";

    private readonly string _databasePath =
        Path.Combine(Path.GetTempPath(), $"trailshop-tests-{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.UseSetting("ConnectionStrings:Shop", $"Data Source={_databasePath}");
    }

    public HttpClient CreateClientAs(string? demoUser)
    {
        var client = CreateClient();
        if (demoUser is not null)
        {
            client.DefaultRequestHeaders.Add(DemoUserHeader, demoUser);
        }

        return client;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        SqliteConnection.ClearAllPools();

        try
        {
            File.Delete(_databasePath);
        }
        catch (IOException)
        {
            // Best effort cleanup of a temp file.
        }
    }
}
