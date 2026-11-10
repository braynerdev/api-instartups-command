namespace Instartups.Command.Application.Common;

/// <summary>
/// Data de referência do negócio. A plataforma atende startups do Porto Digital (Recife),
/// então "hoje" é a data no fuso de Brasília, e não a data UTC do servidor.
/// </summary>
public static class DataReferencia
{
    private const string FusoBrasilia = "America/Sao_Paulo";

    // O Brasil não tem horário de verão desde 2019: se o servidor não tiver a base de fusos,
    // o deslocamento fixo de -3h dá o mesmo resultado.
    private static readonly TimeSpan DeslocamentoBrasilia = TimeSpan.FromHours(-3);

    public static DateOnly HojeBrasilia(DateTime? agoraUtc = null)
    {
        var utc = DateTime.SpecifyKind(agoraUtc ?? DateTime.UtcNow, DateTimeKind.Utc);
        return DateOnly.FromDateTime(ConverterParaBrasilia(utc));
    }

    private static DateTime ConverterParaBrasilia(DateTime utc)
    {
        try
        {
            var fuso = TimeZoneInfo.FindSystemTimeZoneById(FusoBrasilia);
            return TimeZoneInfo.ConvertTimeFromUtc(utc, fuso);
        }
        catch (Exception ex) when (ex is TimeZoneNotFoundException or InvalidTimeZoneException)
        {
            return utc.Add(DeslocamentoBrasilia);
        }
    }
}
