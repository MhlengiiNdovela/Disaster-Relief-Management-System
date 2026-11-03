using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace GiftOfTheGivers.Tests;

public sealed class GiftAppFactory : WebApplicationFactory<Program>
{
    public string DbPath { get; } = Path.Combine(Path.GetTempPath(), $"gift-it-{Guid.NewGuid():N}.db");

    public GiftAppFactory()
    {
        Environment.SetEnvironmentVariable("ConnectionStrings__GiftOfTheGivers", $"Data Source={DbPath}");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:GiftOfTheGivers"] = $"Data Source={DbPath}",
                ["TaxCertificateFunctionUrl"] = ""
            });
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        Environment.SetEnvironmentVariable("ConnectionStrings__GiftOfTheGivers", null);
        SqliteConnection.ClearAllPools();
        if (File.Exists(DbPath))
        {
            File.Delete(DbPath);
        }
    }
}
