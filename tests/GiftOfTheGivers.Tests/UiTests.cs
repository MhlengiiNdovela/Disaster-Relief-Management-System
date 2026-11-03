using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;

namespace GiftOfTheGivers.Tests;

public sealed class RunningSite : IAsyncLifetime
{
    private Process? _process;
    public string Url { get; private set; } = "";
    public string DbPath { get; } = Path.Combine(Path.GetTempPath(), $"gift-ui-{Guid.NewGuid():N}.db");

    public async Task InitializeAsync()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        Url = $"http://127.0.0.1:{port}";

        var repo = FindRepoRoot();
        var configuration = typeof(RunningSite).Assembly.Location.Contains($"{Path.DirectorySeparatorChar}Release{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase)
            ? "Release"
            : "Debug";
        var dll = Path.Combine(repo, "Web", "bin", configuration, "net10.0", "Web.dll");
        if (!File.Exists(dll))
        {
            throw new FileNotFoundException("Build the Web project before the UI tests.", dll);
        }

        var start = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"\"{dll}\"",
            WorkingDirectory = Path.GetDirectoryName(dll)!,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        start.Environment["ASPNETCORE_ENVIRONMENT"] = "Development";
        start.Environment["ASPNETCORE_URLS"] = Url;
        start.Environment["ConnectionStrings__GiftOfTheGivers"] = $"Data Source={DbPath}";
        _process = Process.Start(start) ?? throw new InvalidOperationException("The site process did not start.");

        using var http = new HttpClient();
        for (var attempt = 0; attempt < 40; attempt++)
        {
            if (_process.HasExited)
            {
                throw new InvalidOperationException(await _process.StandardError.ReadToEndAsync());
            }

            try
            {
                var response = await http.GetAsync(Url + "/");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return;
                }
            }
            catch (HttpRequestException)
            {
            }

            await Task.Delay(500);
        }

        throw new TimeoutException("The site did not answer on " + Url);
    }

    public Task DisposeAsync()
    {
        if (_process is { HasExited: false })
        {
            _process.Kill(entireProcessTree: true);
            _process.WaitForExit(5000);
        }

        _process?.Dispose();
        Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
        if (File.Exists(DbPath))
        {
            File.Delete(DbPath);
        }

        return Task.CompletedTask;
    }

    private static string FindRepoRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Web", "Web.csproj")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not find the Web project from the test output folder.");
    }
}

public class UiTests : IClassFixture<RunningSite>
{
    private readonly RunningSite _site;

    public UiTests(RunningSite site)
    {
        _site = site;
    }

    [Fact]
    public void Employee_can_log_in_and_open_the_dashboard()
    {
        using var driver = CreateDriver();
        driver.Navigate().GoToUrl(_site.Url + "/Account/Login");
        driver.FindElement(By.Id("Email")).SendKeys("employee@giftofthegivers.org");
        driver.FindElement(By.Id("Password")).SendKeys("Employee#12345");
        driver.FindElement(By.CssSelector("button.btn-primary")).Click();

        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        wait.Until(browser => browser.PageSource.Contains("employee@giftofthegivers.org"));
        driver.Navigate().GoToUrl(_site.Url + "/Employee");
        wait.Until(browser => browser.PageSource.Contains("Employee dashboard"));
        Assert.Contains("Post a relief project update", driver.PageSource);
    }

    [Fact]
    public void Anonymous_visitor_can_submit_a_donation()
    {
        using var driver = CreateDriver();
        driver.Navigate().GoToUrl(_site.Url + "/Donations");
        driver.FindElement(By.Id("DonorName")).SendKeys("Selenium Donor");
        var amount = driver.FindElement(By.Id("Amount"));
        amount.Clear();
        amount.SendKeys("80");
        driver.FindElement(By.CssSelector("button.btn-primary")).Click();

        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        wait.Until(browser => browser.PageSource.Contains("Donation received"));
        Assert.Contains("Selenium Donor", driver.PageSource);
        Assert.Contains("GOTG-", driver.PageSource);
    }

    private static IWebDriver CreateDriver()
    {
        if (OperatingSystem.IsWindows())
        {
            var edge = new EdgeOptions();
            edge.AddArgument("--headless=new");
            edge.AddArgument("--disable-gpu");
            edge.AddArgument("--window-size=1400,900");
            return new EdgeDriver(edge);
        }

        var chrome = new ChromeOptions();
        chrome.AddArgument("--headless=new");
        chrome.AddArgument("--disable-gpu");
        chrome.AddArgument("--no-sandbox");
        chrome.AddArgument("--window-size=1400,900");
        return new ChromeDriver(chrome);
    }
}
