using Instartups.Command.Api.DTOs;
using Instartups.Command.Api.UnitTests.Builders;
using Instartups.Command.Application.Common.Command;
using Instartups.Command.Domain.Enums;

namespace Instartups.Command.Api.UnitTests.Dtos;

public class PerfilDtosTests
{
    private readonly Guid _userId = Guid.NewGuid();

    [Fact]
    public void CoordenadaDTO_ToCommand_DeveMapearLatitudeELongitude()
    {
        var command = new CoordenadaDTO(-23.5, -46.6).ToCommand();

        command.ShouldBe(new CoordenadaCommand(-23.5, -46.6));
    }

    [Fact]
    public void CadastrarPerfilDTO_ToCommand_DeveMapearCamposEUserId()
    {
        var dto = DtoBuilder.CadastrarPerfil();

        var command = dto.ToCommand(_userId);

        command.ShouldBe(new CadastrarPerfilCommand(dto.Nome, dto.ImagemPerfilUrl, dto.ImagemFundoUrl, new CoordenadaCommand(dto.Coordenada.Latitude, dto.Coordenada.Longitude), _userId));
    }

    [Fact]
    public void CadastrarPerfilDTO_ToCommand_QuandoCoordenadaNula_DeveMapearCoordenadaNula()
    {
        var dto = DtoBuilder.CadastrarPerfil() with { Coordenada = null! };

        dto.ToCommand(_userId).Coordenada.ShouldBeNull();
    }

    [Fact]
    public void EditarPerfilDTO_ToCommand_DeveMapearCamposEUserId()
    {
        var dto = DtoBuilder.EditarPerfil();

        var command = dto.ToCommand(_userId);

        command.ShouldBe(new EditarPerfilCommand(dto.Nome, dto.ImagemPerfilUrl, dto.ImagemFundoUrl, new CoordenadaCommand(dto.Coordenada.Latitude, dto.Coordenada.Longitude), _userId));
    }

    [Fact]
    public void CadastrarPerfilStartupDTO_ToCommand_DeveMapearPerfilEDadosDaStartup()
    {
        var dto = DtoBuilder.CadastrarStartup();

        var command = dto.ToCommand(_userId);

        command.Perfil.ShouldBe(dto.Perfil.ToCommand(_userId));
        command.Pitch.ShouldBe(dto.Pitch);
        command.DataFundacao.ShouldBe(dto.DataFundacao);
        command.TamanhoEquipe.ShouldBe(dto.TamanhoEquipe);
        command.ValorBuscado.ShouldBe(dto.ValorBuscado);
        command.Setor.ShouldBe(SetorStartupEnum.Financas);
    }

    [Theory]
    [InlineData("SETOR_INEXISTENTE")]
    [InlineData("")]
    [InlineData(null)]
    public void CadastrarPerfilStartupDTO_ToCommand_QuandoSetorDesconhecidoOuNulo_DeveMapearParaValorZero(string? setor)
    {
        var dto = DtoBuilder.CadastrarStartup() with { Setor = setor! };

        ((int)dto.ToCommand(_userId).Setor).ShouldBe(0);
    }

    [Fact]
    public void CadastrarPerfilStartupDTO_ToCommand_QuandoPerfilNulo_DeveMapearPerfilNulo()
    {
        var dto = DtoBuilder.CadastrarStartup() with { Perfil = null! };

        dto.ToCommand(_userId).Perfil.ShouldBeNull();
    }

    [Fact]
    public void EditarPerfilStartupDTO_ToCommand_DeveMapearPerfilEDadosDaStartup()
    {
        var dto = DtoBuilder.EditarStartup();

        var command = dto.ToCommand(_userId);

        command.Perfil.ShouldBe(dto.Perfil.ToCommand(_userId));
        command.Pitch.ShouldBe(dto.Pitch);
        command.DataFundacao.ShouldBe(dto.DataFundacao);
        command.TamanhoEquipe.ShouldBe(dto.TamanhoEquipe);
        command.ValorBuscado.ShouldBe(dto.ValorBuscado);
        command.Setor.ShouldBe(SetorStartupEnum.Saude);
    }

    [Theory]
    [InlineData("SETOR_INEXISTENTE")]
    [InlineData("")]
    [InlineData(null)]
    public void EditarPerfilStartupDTO_ToCommand_QuandoSetorDesconhecidoOuNulo_DeveMapearParaValorZero(string? setor)
    {
        var dto = DtoBuilder.EditarStartup() with { Setor = setor! };

        ((int)dto.ToCommand(_userId).Setor).ShouldBe(0);
    }

    [Fact]
    public void CadastrarPerfilInvestidorDTO_ToCommand_DeveMapearPerfilEDadosDoInvestidor()
    {
        var dto = DtoBuilder.CadastrarInvestidor();

        var command = dto.ToCommand(_userId);

        command.Perfil.ShouldBe(dto.Perfil.ToCommand(_userId));
        command.TeseInvestimento.ShouldBe(dto.TeseInvestimento);
        command.TicketMinimo.ShouldBe(dto.TicketMinimo);
        command.TicketMaximo.ShouldBe(dto.TicketMaximo);
    }

    [Fact]
    public void EditarPerfilInvestidorDTO_ToCommand_DeveMapearPerfilEDadosDoInvestidor()
    {
        var dto = DtoBuilder.EditarInvestidor();

        var command = dto.ToCommand(_userId);

        command.Perfil.ShouldBe(dto.Perfil.ToCommand(_userId));
        command.TeseInvestimento.ShouldBe(dto.TeseInvestimento);
        command.TicketMinimo.ShouldBe(dto.TicketMinimo);
        command.TicketMaximo.ShouldBe(dto.TicketMaximo);
    }
}
