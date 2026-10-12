using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Enums;

namespace Instartups.Command.Domain.UnitTests.Entities;

public class MidiaPostagemEntityTests
{
    [Theory]
    [InlineData(TiposMidiaEnum.FOTO)]
    [InlineData(TiposMidiaEnum.VIDEO)]
    public void Criar_DeveGuardarUrlTipoEPostagem(TiposMidiaEnum tipo)
    {
        var postagemId = Guid.NewGuid();

        var midia = MidiaPostagemEntity.Criar("https://cdn.instartups.com/midia.png", tipo, postagemId);

        midia.UrlMidia.ShouldBe("https://cdn.instartups.com/midia.png");
        midia.TipoMidia.ShouldBe(tipo);
        midia.PostagemId.ShouldBe(postagemId);
        midia.Id.ShouldNotBe(Guid.Empty);
    }
}
