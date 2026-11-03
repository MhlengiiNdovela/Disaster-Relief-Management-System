using GiftOfTheGivers.Helpers;

namespace GiftOfTheGivers.Tests;

public class DonationRulesTests
{
    [Fact]
    public void Donation_validation_accepts_a_named_gift_in_a_supported_currency()
    {
        Assert.True(DonationRules.IsValid("Thandi Mkhize", 250m, "USD"));
        Assert.False(DonationRules.IsValid(" ", 250m, "USD"));
        Assert.False(DonationRules.IsValid("Thandi Mkhize", 0m, "ZAR"));
        Assert.False(DonationRules.IsValid("Thandi Mkhize", 100m, "GBP"));
    }

    [Fact]
    public void Donation_total_is_the_sum_of_the_recorded_amounts()
    {
        var total = DonationRules.CalculateTotal([100m, 50.5m, 25m]);

        Assert.Equal(175.5m, total);
        Assert.Equal(0m, DonationRules.CalculateTotal([]));
    }

    [Fact]
    public void Certificate_number_uses_the_year_and_a_six_digit_donation_id()
    {
        var number = CertificateFormatter.FormatNumber(12, new DateOnly(2026, 11, 3));

        Assert.Equal("GOTG-2026-000012", number);
    }
}
