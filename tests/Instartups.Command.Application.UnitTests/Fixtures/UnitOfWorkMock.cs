using Instartups.Command.Application.Interfaces;
using Instartups.Command.Application.Interfaces.Repositories;
using Instartups.Command.Domain.Entities;

namespace Instartups.Command.Application.UnitTests.Fixtures;

public sealed class UnitOfWorkMock
{
    public IUnitOfWork UnitOfWork { get; } = Substitute.For<IUnitOfWork>();
    public IPerfilRepository Perfil { get; } = Substitute.For<IPerfilRepository>();
    public IPerfilSeguidorRepository PerfilSeguidor { get; } = Substitute.For<IPerfilSeguidorRepository>();
    public IPostagemRepository Postagem { get; } = Substitute.For<IPostagemRepository>();
    public ICurtidaPostagemRepository CurtidaPostagem { get; } = Substitute.For<ICurtidaPostagemRepository>();

    public UnitOfWorkMock()
    {
        UnitOfWork.Perfil.Returns(Perfil);
        UnitOfWork.PerfilSeguidor.Returns(PerfilSeguidor);
        UnitOfWork.Postagem.Returns(Postagem);
        UnitOfWork.CurtidaPostagem.Returns(CurtidaPostagem);
    }

    public UnitOfWorkMock ComPerfilDoUsuario(PerfilEntity perfil)
    {
        Perfil.ObterPorUsuarioIdAsync(perfil.UsuarioId, Arg.Any<CancellationToken>()).Returns(perfil);
        return this;
    }

    public UnitOfWorkMock SemPerfilDoUsuario(Guid usuarioId)
    {
        Perfil.ObterPorUsuarioIdAsync(usuarioId, Arg.Any<CancellationToken>()).Returns((PerfilEntity?)null);
        return this;
    }

    public async Task DeveTerCommitadoUmaVez(CancellationToken ct)
    {
        await UnitOfWork.Received(1).CommitAsync(ct);
    }

    public async Task NaoDeveTerCommitado()
    {
        await UnitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }
}
