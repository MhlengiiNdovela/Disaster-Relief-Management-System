namespace GiftOfTheGivers.Helpers;

public static class CertificateFormatter
{
    public static string FormatNumber(int donationId, DateOnly donationDate)
    {
        return $"GOTG-{donationDate:yyyy}-{donationId:D6}";
    }
}
