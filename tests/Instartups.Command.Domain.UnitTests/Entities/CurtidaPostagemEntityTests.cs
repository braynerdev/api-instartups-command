using Instartups.Command.Domain.Entities;

namespace Instartups.Command.Domain.UnitTests.Entities;

public class CurtidaPostagemEntityTests
{
    [Fact]
    public void Criar_DeveVincularPostagemEPerfilAutorDaCurtida()
    {
        var postagemId = Guid.NewGuid();
        var perfilId = Guid.NewGuid();

        var curtida = CurtidaPostagemEntity.Criar(postagemId, perfilId);

        curtida.PostagemId.ShouldBe(postagemId);
        curtida.PerfilAutorCurtidaId.ShouldBe(perfilId);
        curtida.Id.ShouldNotBe(Guid.Empty);
        curtida.Ativo.ShouldBeTrue();
    }
}
