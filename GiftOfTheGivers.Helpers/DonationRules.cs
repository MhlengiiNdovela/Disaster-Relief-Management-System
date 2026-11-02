namespace GiftOfTheGivers.Helpers;

public static class DonationRules
{
    public static readonly string[] Currencies = ["ZAR", "USD", "EUR"];

    public static bool IsValid(string? donorName, decimal amount, string? currency)
    {
        return !string.IsNullOrWhiteSpace(donorName)
            && amount > 0
            && amount <= 10_000_000
            && currency is not null
            && Currencies.Contains(currency, StringComparer.OrdinalIgnoreCase);
    }

    public static decimal CalculateTotal(IEnumerable<decimal> amounts)
    {
        return amounts.Sum();
    }
}
