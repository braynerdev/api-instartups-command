using Instartups.Command.Api.DTOs;
using Instartups.Command.Api.UnitTests.Builders;
using Instartups.Command.Application.Common.Command;
using Instartups.Command.Domain.Enums;

namespace Instartups.Command.Api.UnitTests.Dtos;

public class PostagemDtosTests
{
    private readonly Guid _userId = Guid.NewGuid();

    #region MidiaDTO

    [Theory]
    [InlineData("FOTO", TiposMidiaEnum.FOTO)]
    [InlineData("foto", TiposMidiaEnum.FOTO)]
    [InlineData("VIDEO", TiposMidiaEnum.VIDEO)]
    [InlineData(" video ", TiposMidiaEnum.VIDEO)]
    public void MidiaDTO_ToCommand_DeveMapearUrlETipo(string tipo, TiposMidiaEnum esperado)
    {
        var dto = new MidiaDTO("https://cdn.instartups.com/midia.png", tipo);

        var command = dto.ToCommand();

        command.ShouldBe(new MidiaCommand("https://cdn.instartups.com/midia.png", esperado));
    }

    [Theory]
    [InlineData("AUDIO")]
    [InlineData("")]
    public void MidiaDTO_ToCommand_QuandoTipoDesconhecido_DeveMapearParaValorZero(string tipo)
    {
        var command = new MidiaDTO("https://cdn.instartups.com/midia.png", tipo).ToCommand();

        ((int)command.Tipo).ShouldBe(0);
    }

    [Fact]
    public void MidiaDTO_ToCommand_QuandoTipoNulo_DeveMapearParaValorZero()
    {
        var dto = new MidiaDTO("https://cdn.instartups.com/midia.png", null!);

        ((int)dto.ToCommand().Tipo).ShouldBe(0);
    }

    #endregion

    #region CadastrarPostagemDTO

    [Fact]
    public void CadastrarPostagemDTO_ToCommand_DeveMapearDescricaoAutorEMidias()
    {
        var dto = DtoBuilder.CadastrarPostagem();

        var command = dto.ToCommand(_userId);

        command.Descricao.ShouldBe(dto.Descricao);
        command.AutorId.ShouldBe(_userId);
        command.Midias.ShouldBe(
        [
            new MidiaCommand(DtoBuilder.Midia(indice: 0).Url, TiposMidiaEnum.FOTO),
            new MidiaCommand(DtoBuilder.Midia(indice: 1).Url, TiposMidiaEnum.VIDEO)
        ]);
    }

    [Fact]
    public void CadastrarPostagemDTO_ToCommand_QuandoMidiasNulas_DeveRetornarListaVazia()
    {
        var dto = DtoBuilder.CadastrarPostagem() with { Midias = null };

        var command = dto.ToCommand(_userId);

        command.Midias.ShouldNotBeNull();
        command.Midias.ShouldBeEmpty();
    }

    #endregion

    #region EditarPostagemDTO

    [Fact]
    public void EditarPostagemDTO_ToCommand_DeveMapearTodosOsCampos()
    {
        var dto = DtoBuilder.EditarPostagem();
        var postagemId = Guid.NewGuid();

        var command = dto.ToCommand(postagemId, _userId);

        command.PostagemId.ShouldBe(postagemId);
        command.UserId.ShouldBe(_userId);
        command.Descricao.ShouldBe(dto.Descricao);
        command.MidiasRemovidas.ShouldBe(dto.MidiasRemovidas!);
        command.MidiasAdicionadas.ShouldBe([new MidiaCommand(DtoBuilder.Midia(indice: 2).Url, TiposMidiaEnum.VIDEO)]);
    }

    [Fact]
    public void EditarPostagemDTO_ToCommand_QuandoListasNulas_DeveRetornarListasVazias()
    {
        var dto = DtoBuilder.EditarPostagem() with { MidiasRemovidas = null, MidiasAdicionadas = null };

        var command = dto.ToCommand(Guid.NewGuid(), _userId);

        command.MidiasRemovidas.ShouldBeEmpty();
        command.MidiasAdicionadas.ShouldBeEmpty();
    }

    #endregion
}
