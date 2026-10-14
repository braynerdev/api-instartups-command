using System.Net;
using Instartups.Command.Api.DTOs;
using Instartups.Command.Api.DTOs.Postagem;
using Instartups.Command.Api.IntegrationTests.Fixtures;
using Instartups.Command.Api.UnitTests.Builders;
using Instartups.Command.Domain.Constants;
using Instartups.Command.Domain.Enums;
using Instartups.Command.Tests.Common.Builders.Entities;
using Microsoft.EntityFrameworkCore;

namespace Instartups.Command.Api.IntegrationTests.Postagem;

public class CadastrarPostagemTests(InstartupsApiFactory factory) : IntegrationTestBase(factory)
{
    private readonly Guid _usuarioId = Guid.NewGuid();

    [Fact]
    public async Task Cadastrar_ComMidias_DeveRetornar201EPersistirPostagemComMidias()
    {
        var autor = PerfilBuilder.Startup().ComUsuarioId(_usuarioId).Build();
        await SemearAsync(autor);
        var dto = DtoBuilder.CadastrarPostagem();

        var resposta = await PostAsync(CriarCliente(_usuarioId), Rotas.Postagem.Base, dto);

        var id = await resposta.DeveSerCriadoAsync(MensagensSucessoConst.PostagemCadastrada);
        var postagem = await ConsultarAsync(db => db.Postagem.Include(p => p.MidiasPostagem).SingleAsync(p => p.Id == id, Ct));
        postagem.Descricao.ShouldBe(dto.Descricao);
        postagem.AutorId.ShouldBe(autor.Id);
        postagem.TotalCurtidas.Total.ShouldBe(0);
        postagem.Ativo.ShouldBeTrue();

        postagem.MidiasPostagem.Count.ShouldBe(2);
        postagem.MidiasPostagem.ShouldAllBe(m => m.PostagemId == id);
        postagem.MidiasPostagem.ShouldContain(m => m.UrlMidia == "https://cdn.instartups.com/midia-0.png" && m.TipoMidia == TiposMidiaEnum.Foto);
        // "video" em minúsculas deve ser aceito.
        postagem.MidiasPostagem.ShouldContain(m => m.UrlMidia == "https://cdn.instartups.com/midia-1.png" && m.TipoMidia == TiposMidiaEnum.Video);
    }

    [Fact]
    public async Task Cadastrar_SemMidias_DeveRetornar201EPersistirPostagemSemMidias()
    {
        await SemearAsync(PerfilBuilder.Investidor().ComUsuarioId(_usuarioId).Build());

        var resposta = await PostAsync(CriarCliente(_usuarioId), Rotas.Postagem.Base, new CadastrarPostagemDTO("Só texto.", null));

        var id = await resposta.DeveSerCriadoAsync(MensagensSucessoConst.PostagemCadastrada);
        (await ConsultarAsync(db => db.MidiaPostagem.CountAsync(m => m.PostagemId == id, Ct))).ShouldBe(0);
    }

    [Fact]
    public async Task Cadastrar_ComDezMidias_DeveRetornar201()
    {
        await SemearAsync(PerfilBuilder.Startup().ComUsuarioId(_usuarioId).Build());
        var dto = new CadastrarPostagemDTO("Dez mídias.", Enumerable.Range(0, 10).Select(i => DtoBuilder.Midia("FOTO", i)).ToList());

        var resposta = await PostAsync(CriarCliente(_usuarioId), Rotas.Postagem.Base, dto);

        var id = await resposta.DeveSerCriadoAsync(MensagensSucessoConst.PostagemCadastrada);
        (await ConsultarAsync(db => db.MidiaPostagem.CountAsync(m => m.PostagemId == id, Ct))).ShouldBe(10);
    }

    [Fact]
    public async Task Cadastrar_ComMaisDeDezMidias_DeveRetornar422ENaoPersistir()
    {
        await SemearAsync(PerfilBuilder.Startup().ComUsuarioId(_usuarioId).Build());
        var dto = new CadastrarPostagemDTO("Onze mídias.", Enumerable.Range(0, 11).Select(i => DtoBuilder.Midia("FOTO", i)).ToList());

        var resposta = await PostAsync(CriarCliente(_usuarioId), Rotas.Postagem.Base, dto);

        await resposta.DeveSerErroAsync(HttpStatusCode.UnprocessableEntity, "Não é possível adicionar mais de 10 mídias a uma postagem.");
        (await ConsultarAsync(db => db.Postagem.CountAsync(Ct))).ShouldBe(0);
        (await ConsultarAsync(db => db.MidiaPostagem.CountAsync(Ct))).ShouldBe(0);
    }

    [Fact]
    public async Task Cadastrar_QuandoUsuarioNaoPossuiPerfil_DeveRetornar404ENaoPersistir()
    {
        var resposta = await PostAsync(CriarCliente(_usuarioId), Rotas.Postagem.Base, DtoBuilder.CadastrarPostagem());

        await resposta.DeveSerErroAsync(HttpStatusCode.NotFound, "Perfil não encontrado.");
        (await ConsultarAsync(db => db.Postagem.CountAsync(Ct))).ShouldBe(0);
    }

    [Fact]
    public async Task Cadastrar_QuandoPerfilDesativado_DeveRetornar422ENaoPersistir()
    {
        await SemearAsync(PerfilBuilder.Startup().ComUsuarioId(_usuarioId).Desativado().Build());

        var resposta = await PostAsync(CriarCliente(_usuarioId), Rotas.Postagem.Base, DtoBuilder.CadastrarPostagem());

        await resposta.DeveSerErroAsync(HttpStatusCode.UnprocessableEntity, "Não é possível realizar esta ação porque o perfil está desativado.");
        (await ConsultarAsync(db => db.Postagem.CountAsync(Ct))).ShouldBe(0);
    }

    [Theory]
    [InlineData("descricao-vazia", "Descricao", CodigosErroConst.CampoObrigatorio)]
    [InlineData("descricao-longa", "Descricao", CodigosErroConst.TamanhoMaximo)]
    [InlineData("midia-url-vazia", "Midias[0].Url", CodigosErroConst.CampoObrigatorio)]
    [InlineData("midia-url-invalida", "Midias[0].Url", CodigosErroConst.UrlInvalida)]
    [InlineData("midia-url-longa", "Midias[1].Url", CodigosErroConst.TamanhoMaximo)]
    [InlineData("midia-tipo-invalido", "Midias[0].Tipo", CodigosErroConst.ValorInvalidoEnum)]
    public async Task Cadastrar_ComPayloadInvalido_DeveRetornar400ENaoPersistir(string cenario, string campo, string codigo)
    {
        await SemearAsync(PerfilBuilder.Startup().ComUsuarioId(_usuarioId).Build());

        var resposta = await PostAsync(CriarCliente(_usuarioId), Rotas.Postagem.Base, PayloadInvalido(cenario));

        await resposta.DeveSerErroDeValidacaoAsync(campo, codigo);
        (await ConsultarAsync(db => db.Postagem.CountAsync(Ct))).ShouldBe(0);
    }

    private static CadastrarPostagemDTO PayloadInvalido(string cenario)
    {
        var valido = DtoBuilder.CadastrarPostagem();
        var foto = DtoBuilder.Midia("FOTO");

        return cenario switch
        {
            "descricao-vazia" => valido with { Descricao = "" },
            "descricao-longa" => valido with { Descricao = new string('a', TamanhosColunasConst.Postagem.Descricao + 1) },
            "midia-url-vazia" => valido with { Midias = [foto with { Url = "" }] },
            "midia-url-invalida" => valido with { Midias = [foto with { Url = "javascript:alert(1)" }] },
            "midia-url-longa" => valido with { Midias = [foto, foto with { Url = "https://cdn.instartups.com/" + new string('a', TamanhosColunasConst.MidiaPostagem.Url) }] },
            "midia-tipo-invalido" => valido with { Midias = [new MidiaDTO("https://cdn.instartups.com/audio.mp3", "AUDIO")] },
            _ => throw new ArgumentOutOfRangeException(nameof(cenario), cenario, null)
        };
    }
}
