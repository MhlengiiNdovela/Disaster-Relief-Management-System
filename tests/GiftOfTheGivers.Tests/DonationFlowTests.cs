using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Service.Data;

namespace GiftOfTheGivers.Tests;

public class DonationFlowTests : IClassFixture<GiftAppFactory>
{
    private readonly GiftAppFactory _factory;

    public DonationFlowTests(GiftAppFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Donation_form_is_stored_and_a_certificate_is_generated()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var page = await client.GetAsync("/Donations");
        var html = await page.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, page.StatusCode);

        var token = Regex.Match(html, "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"").Groups[1].Value;
        var projectId = Regex.Match(html, "option value=\"(\\d+)\"").Groups[1].Value;
        Assert.False(string.IsNullOrWhiteSpace(token));
        Assert.False(string.IsNullOrWhiteSpace(projectId));

        var form = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["DonorName"] = "Flow Donor",
            ["Email"] = "flow.donor@example.com",
            ["Amount"] = "150",
            ["Currency"] = "EUR",
            ["ReliefProjectId"] = projectId,
            ["IsRecurring"] = "true"
        });
        var post = await client.PostAsync("/Donations", form);

        Assert.Equal(HttpStatusCode.Redirect, post.StatusCode);
        var location = post.Headers.Location!.ToString();
        Assert.Contains("/Donations/Receipt/", location);

        var receipt = await client.GetAsync(location);
        var receiptHtml = await receipt.Content.ReadAsStringAsync();
        Assert.Contains("GOTG-", receiptHtml);
        Assert.Contains("Flow Donor", receiptHtml);

        var id = Regex.Match(location, @"(\d+)$").Groups[1].Value;
        var pdf = await client.GetAsync($"/Donations/Certificate/{id}");
        var bytes = await pdf.Content.ReadAsByteArrayAsync();
        Assert.Equal(HttpStatusCode.OK, pdf.StatusCode);
        Assert.Equal("%PDF", System.Text.Encoding.ASCII.GetString(bytes, 0, 4));

        var options = new DbContextOptionsBuilder<GiftDbContext>()
            .UseSqlite($"Data Source={_factory.DbPath}")
            .Options;
        using var db = new GiftDbContext(options);
        var saved = Assert.Single(db.Donations);
        Assert.Equal("Flow Donor", saved.Description);
        Assert.Equal(150m, saved.Amount);
        Assert.Equal("EUR", saved.Currency);
        Assert.Equal("Recurring", saved.DonationType);
    }

    [Fact]
    public async Task Five_concurrent_visitors_can_open_the_public_home_page()
    {
        var client = _factory.CreateClient();
        var started = System.Diagnostics.Stopwatch.StartNew();
        var responses = await Task.WhenAll(Enumerable.Range(0, 5).Select(_ => client.GetAsync("/")));
        started.Stop();

        Assert.All(responses, response => Assert.Equal(HttpStatusCode.OK, response.StatusCode));
        Assert.True(started.Elapsed < TimeSpan.FromSeconds(30));

        var reportDirectory = Path.Combine(Path.GetTempPath(), "poe-shots");
        Directory.CreateDirectory(reportDirectory);
        await File.WriteAllTextAsync(
            Path.Combine(reportDirectory, "load-test.txt"),
            $"Concurrent users: 5{Environment.NewLine}Target: GET /{Environment.NewLine}Status codes: {string.Join(", ", responses.Select(response => (int)response.StatusCode))}{Environment.NewLine}Elapsed: {started.ElapsedMilliseconds} ms");
    }
}
