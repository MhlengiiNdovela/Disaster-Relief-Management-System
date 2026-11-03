using System.Net.Http.Json;
using GiftOfTheGivers.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Services;

namespace Web.Controllers;

[AllowAnonymous]
[Route("tax-certificates")]
public sealed class TaxCertificatesController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public TaxCertificatesController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("generate")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Generate([FromBody] DonationCertificateRequest donation, CancellationToken cancellationToken)
    {
        if (!DonationRules.IsValid(donation.DonorName, donation.Amount, donation.Currency))
        {
            return BadRequest("A donor name and a positive donation amount are required.");
        }

        var client = _httpClientFactory.CreateClient("TaxCertificateFunction");
        if (client.BaseAddress is not null)
        {
            try
            {
                using var response = await client.PostAsJsonAsync(string.Empty, donation, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    var remotePdf = await response.Content.ReadAsByteArrayAsync(cancellationToken);
                    return File(remotePdf, "application/pdf", "donation-tax-certificate.pdf");
                }
            }
            catch (HttpRequestException)
            {
                // The external function host is unavailable. Generate the certificate in this app.
            }
        }

        var pdf = TaxCertificatePdf.Create(donation);
        return File(pdf, "application/pdf", "donation-tax-certificate.pdf");
    }
}