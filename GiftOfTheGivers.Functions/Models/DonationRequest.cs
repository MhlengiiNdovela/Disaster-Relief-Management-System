namespace GiftOfTheGivers.Functions.Models;

public sealed class DonationRequest
{
    public string DonorName { get; init; } = string.Empty;

    public decimal Amount { get; init; }

    public string Currency { get; init; } = "ZAR";

    public DateOnly DonationDate { get; init; } = DateOnly.FromDateTime(DateTime.UtcNow);

    public string ReliefProjectTitle { get; init; } = string.Empty;
}