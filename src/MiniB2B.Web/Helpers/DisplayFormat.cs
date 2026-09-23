using System.Globalization;

namespace MiniB2B.Web.Helpers;

public static class DisplayFormat
{
    private static readonly CultureInfo TurkishCulture = new("tr-TR");
    private static readonly TimeZoneInfo TurkeyTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("Europe/Istanbul");

    public static string Money(decimal value) =>
        value.ToString("C2", TurkishCulture);

    public static string Date(DateTime utcValue) =>
        TimeZoneInfo.ConvertTimeFromUtc(utcValue, TurkeyTimeZone)
            .ToString("dd.MM.yyyy HH:mm", TurkishCulture);
}