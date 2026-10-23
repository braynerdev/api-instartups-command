using Instartups.Command.Application.UnitTests.Fixtures;
using Instartups.Command.Application.UseCases.CadastrarPostagem;
using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Enums;
using Instartups.Command.Domain.Exceptions;
using Instartups.Command.Tests.Common.Builders.Commands;
using Instartups.Command.Tests.Common.Builders.Entities;

namespace Instartups.Command.Application.UnitTests.UseCases.CadastrarPostagem;

public class CadastrarPostagemCommandHandlerTests
{
    private readonly UnitOfWorkMock _uow = new();
    private readonly CancellationToken _ct = TestContext.Current.CancellationToken;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly CadastrarPostagemCommandHandler _handler;
    private PostagemEntity? _postagemAdicionada;

    public CadastrarPostagemCommandHandlerTests()
    {
        _handler = new CadastrarPostagemCommandHandler(_uow.UnitOfWork);
        _uow.Postagem.Add(Arg.Do<PostagemEntity>(p => _postagemAdicionada = p));
    }

    [Fact]
    public async Task Handle_QuandoAutorAtivo_DeveAdicionarPostagemVinculadaAoPerfilDoUsuario()
    {
        var perfil = PerfilBuilder.Startup().ComUsuarioId(_userId).Build();
        _uow.ComPerfilDoUsuario(perfil);
        var command = PostagemCommandBuilder.CadastrarPostagem(_userId, quantidadeMidias: 0);

        await _handler.Handle(command, _ct);

        _postagemAdicionada.ShouldNotBeNull();
        _postagemAdicionada.Descricao.ShouldBe(command.Descricao);
        _postagemAdicionada.AutorId.ShouldBe(perfil.Id);
        _postagemAdicionada.Ativo.ShouldBeTrue();
        _postagemAdicionada.MidiasPostagem.ShouldBeEmpty();
    }

    [Fact]
    public async Task Handle_QuandoCommandTemMidias_DeveAdicionarMidiasNaPostagem()
    {
        _uow.ComPerfilDoUsuario(PerfilBuilder.Startup().ComUsuarioId(_userId).Build());
        var command = PostagemCommandBuilder.CadastrarPostagem(_userId) with
        {
            Midias = [PostagemCommandBuilder.Midia(0, TiposMidiaEnum.Foto), PostagemCommandBuilder.Midia(1, TiposMidiaEnum.Video)]
        };

        await _handler.Handle(command, _ct);

        _postagemAdicionada!.MidiasPostagem.Select(m => (m.UrlMidia, m.TipoMidia)).ShouldBe(
        [
            (PostagemCommandBuilder.Midia(0).Url, TiposMidiaEnum.Foto),
            (PostagemCommandBuilder.Midia(1).Url, TiposMidiaEnum.Video)
        ]);
        _postagemAdicionada.MidiasPostagem.ShouldAllBe(m => m.PostagemId == _postagemAdicionada.Id);
    }

    [Fact]
    public async Task Handle_QuandoAutorAtivo_DeveRetornarIdDaPostagemECommitar()
    {
        _uow.ComPerfilDoUsuario(PerfilBuilder.Startup().ComUsuarioId(_userId).Build());

        var resposta = await _handler.Handle(PostagemCommandBuilder.CadastrarPostagem(_userId), _ct);

        resposta.Id.ShouldBe(_postagemAdicionada!.Id);
        await _uow.DeveTerCommitadoUmaVez(_ct);
    }

    [Fact]
    public async Task Handle_QuandoPerfilNaoEncontrado_DeveLancarPerfilNaoEncontradoException()
    {
        _uow.SemPerfilDoUsuario(_userId);

        var acao = () => _handler.Handle(PostagemCommandBuilder.CadastrarPostagem(_userId), _ct);

        await acao.ShouldThrowAsync<PerfilNaoEncontradoException>();
        _uow.Postagem.DidNotReceive().Add(Arg.Any<PostagemEntity>());
        await _uow.NaoDeveTerCommitado();
    }

    [Fact]
    public async Task Handle_QuandoPerfilDesativado_DeveLancarPerfilDesativadoException()
    {
        _uow.ComPerfilDoUsuario(PerfilBuilder.Startup().ComUsuarioId(_userId).Desativado().Build());

        var acao = () => _handler.Handle(PostagemCommandBuilder.CadastrarPostagem(_userId), _ct);

        await acao.ShouldThrowAsync<PerfilDesativadoException>();
        _uow.Postagem.DidNotReceive().Add(Arg.Any<PostagemEntity>());
        await _uow.NaoDeveTerCommitado();
    }

    [Fact]
    public async Task Handle_QuandoCommandTemMaisDeDezMidias_DeveLancarLimiteMidiasPostagemExcedidoException()
    {
        _uow.ComPerfilDoUsuario(PerfilBuilder.Startup().ComUsuarioId(_userId).Build());

        var acao = () => _handler.Handle(PostagemCommandBuilder.CadastrarPostagem(_userId, quantidadeMidias: 11), _ct);

        await acao.ShouldThrowAsync<LimiteMidiasPostagemExcedidoException>();
        _uow.Postagem.DidNotReceive().Add(Arg.Any<PostagemEntity>());
        await _uow.NaoDeveTerCommitado();
    }
}
