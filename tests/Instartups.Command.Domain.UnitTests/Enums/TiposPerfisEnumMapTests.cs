using Instartups.Command.Domain.Enums;
using Instartups.Command.Domain.Enums.Mapear;

namespace Instartups.Command.Domain.UnitTests.Enums;

public class TiposPerfisEnumMapTests
{
    [Theory]
    [InlineData(TiposPerfisEnum.Startup, "STARTUP")]
    [InlineData(TiposPerfisEnum.Investidor, "INVESTIDOR")]
    public void Map_QuandoTipoValido_DeveRetornarDescricao(TiposPerfisEnum tipo, string esperado)
    {
        tipo.Map().ShouldBe(esperado);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(99)]
    public void Map_QuandoTipoDesconhecido_DeveRetornarAdmin(int valor)
    {
        ((TiposPerfisEnum)valor).Map().ShouldBe("ADMIN");
    }
}
