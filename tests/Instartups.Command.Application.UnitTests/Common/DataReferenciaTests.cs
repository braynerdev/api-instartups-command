using Instartups.Command.Application.Common;

namespace Instartups.Command.Application.UnitTests.Common;

public class DataReferenciaTests
{
    [Theory]
    [InlineData("2026-11-10T00:59:00", "2026-11-09")] // 21:59 em Brasília: ainda é dia 9 (caso do DEF-03)
    [InlineData("2026-11-10T02:59:59", "2026-11-09")] // 23:59:59 em Brasília
    [InlineData("2026-11-10T03:00:00", "2026-11-10")] // meia-noite em Brasília
    [InlineData("2026-11-10T15:00:00", "2026-11-10")] // meio-dia em Brasília
    public void HojeBrasilia_DeveConverterAHoraUtcParaADataDeBrasilia(string agoraUtc, string esperado)
    {
        var agora = DateTime.SpecifyKind(DateTime.Parse(agoraUtc, System.Globalization.CultureInfo.InvariantCulture), DateTimeKind.Utc);

        DataReferencia.HojeBrasilia(agora).ShouldBe(DateOnly.Parse(esperado, System.Globalization.CultureInfo.InvariantCulture));
    }

    [Fact]
    public void HojeBrasilia_SemParametro_DeveUsarAHoraAtual()
    {
        var antes = DataReferencia.HojeBrasilia(DateTime.UtcNow);
        var hoje = DataReferencia.HojeBrasilia();
        var depois = DataReferencia.HojeBrasilia(DateTime.UtcNow);

        hoje.ShouldBeInRange(antes, depois);
    }
}
