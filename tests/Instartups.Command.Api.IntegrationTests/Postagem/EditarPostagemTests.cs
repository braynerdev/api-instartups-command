using System.Net;
using Instartups.Command.Api.DTOs.Postagem;
using Instartups.Command.Api.IntegrationTests.Fixtures;
using Instartups.Command.Api.UnitTests.Builders;
using Instartups.Command.Domain.Constants;
using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Enums;
using Instartups.Command.Tests.Common.Builders.Entities;
using Microsoft.EntityFrameworkCore;

namespace Instartups.Command.Api.IntegrationTests.Postagem;

public class EditarPostagemTests(InstartupsApiFactory factory) : IntegrationTestBase(factory)
{
    private readonly Guid _usuarioId = Guid.NewGuid();

    [Fact]
    public async Task Editar_ComDescricaoEMidias_DeveRetornar204EPersistirAlteracoes()
    {
        var postagem = await SemearPostagemAsync(midias: 2);
        var removida = postagem.MidiasPostagem.Single(m => m.UrlMidia.EndsWith("midia-0.png"));
        var mantida = postagem.MidiasPostagem.Single(m => m.UrlMidia.EndsWith("midia-1.png"));
        var dto = new EditarPostagemDTO("Descrição editada.", [removida.Id], [DtoBuilder.Midia("VIDEO", 9)]);

        var resposta = await PutAsync(CriarCliente(_usuarioId), Rotas.Postagem.PorId(postagem.Id), dto);

        await resposta.DeveSerNoContentAsync();
        var persistida = await ObterPostagemAsync(postagem.Id);
        persistida.Descricao.ShouldBe("Descrição editada.");
        persistida.DataAtualizacao.ShouldNotBeNull();
        persistida.MidiasPostagem.Count.ShouldBe(2);
        persistida.MidiasPostagem.ShouldContain(m => m.Id == mantida.Id && m.TipoMidia == TiposMidiaEnum.Foto);
        persistida.MidiasPostagem.ShouldContain(m => m.UrlMidia == "https://cdn.instartups.com/midia-9.png" && m.TipoMidia == TiposMidiaEnum.Video);
        persistida.MidiasPostagem.ShouldNotContain(m => m.Id == removida.Id);
    }

    [Fact]
    public async Task Editar_SomenteDescricao_DeveRetornar204EManterMidias()
    {
        var postagem = await SemearPostagemAsync(midias: 3);

        var resposta = await PutAsync(CriarCliente(_usuarioId), Rotas.Postagem.PorId(postagem.Id), new EditarPostagemDTO("Nova descrição.", null, null));

        await resposta.DeveSerNoContentAsync();
        var persistida = await ObterPostagemAsync(postagem.Id);
        persistida.Descricao.ShouldBe("Nova descrição.");
        persistida.MidiasPostagem.Count.ShouldBe(3);
    }

    [Fact]
    public async Task Editar_RemovendoTodasAsMidias_DeveRetornar204EApagarMidias()
    {
        var postagem = await SemearPostagemAsync(midias: 2);
        var ids = postagem.MidiasPostagem.Select(m => m.Id).ToList();

        var resposta = await PutAsync(CriarCliente(_usuarioId), Rotas.Postagem.PorId(postagem.Id), new EditarPostagemDTO("Sem mídias.", ids, null));

        await resposta.DeveSerNoContentAsync();
        (await ConsultarAsync(db => db.MidiaPostagem.CountAsync(Ct))).ShouldBe(0);
    }

    [Fact]
    public async Task Editar_TrocandoMidiaComLimiteCheio_DeveRetornar204()
    {
        var postagem = await SemearPostagemAsync(midias: 10);
        var removida = postagem.MidiasPostagem.First();
        var dto = new EditarPostagemDTO("Troca.", [removida.Id], [DtoBuilder.Midia("FOTO", 10)]);

        var resposta = await PutAsync(CriarCliente(_usuarioId), Rotas.Postagem.PorId(postagem.Id), dto);

        await resposta.DeveSerNoContentAsync();
        var persistida = await ObterPostagemAsync(postagem.Id);
        persistida.MidiasPostagem.Count.ShouldBe(10);
        persistida.MidiasPostagem.ShouldNotContain(m => m.Id == removida.Id);
    }

    [Fact]
    public async Task Editar_ExcedendoLimiteDeMidias_DeveRetornar422ENaoPersistirAlteracoes()
    {
        var postagem = await SemearPostagemAsync(midias: 10);
        var dto = new EditarPostagemDTO("Não deve salvar.", null, [DtoBuilder.Midia("FOTO", 10)]);

        var resposta = await PutAsync(CriarCliente(_usuarioId), Rotas.Postagem.PorId(postagem.Id), dto);

        await resposta.DeveSerErroAsync(HttpStatusCode.UnprocessableEntity, "Não é possível adicionar mais de 10 mídias a uma postagem.");
        var persistida = await ObterPostagemAsync(postagem.Id);
        persistida.Descricao.ShouldBe(postagem.Descricao);
        persistida.MidiasPostagem.Count.ShouldBe(10);
    }

    [Fact]
    public async Task Editar_RemovendoMidiaInexistente_DeveRetornar404ENaoPersistirAlteracoes()
    {
        var postagem = await SemearPostagemAsync(midias: 1);
        var dto = new EditarPostagemDTO("Não deve salvar.", [Guid.NewGuid()], null);

        var resposta = await PutAsync(CriarCliente(_usuarioId), Rotas.Postagem.PorId(postagem.Id), dto);

        await resposta.DeveSerErroAsync(HttpStatusCode.NotFound, "Mídia da postagem não encontrada.");
        var persistida = await ObterPostagemAsync(postagem.Id);
        persistida.Descricao.ShouldBe(postagem.Descricao);
        persistida.MidiasPostagem.Count.ShouldBe(1);
    }

    [Fact]
    public async Task Editar_RemovendoMidiaDeOutraPostagem_DeveRetornar404EManterMidiaAlheia()
    {
        var postagem = await SemearPostagemAsync(midias: 1);
        var alheia = PostagemBuilder.Nova().ComMidias(1).Build();
        await SemearAsync(alheia);
        var midiaAlheia = alheia.MidiasPostagem.Single();

        var resposta = await PutAsync(CriarCliente(_usuarioId), Rotas.Postagem.PorId(postagem.Id), new EditarPostagemDTO("x", [midiaAlheia.Id], null));

        await resposta.DeveSerErroAsync(HttpStatusCode.NotFound, "Mídia da postagem não encontrada.");
        (await ConsultarAsync(db => db.MidiaPostagem.AnyAsync(m => m.Id == midiaAlheia.Id, Ct))).ShouldBeTrue();
    }

    [Fact]
    public async Task Editar_QuandoPostagemNaoExiste_DeveRetornar404()
    {
        await SemearAsync(PerfilBuilder.Startup().ComUsuarioId(_usuarioId).Build());

        var resposta = await PutAsync(CriarCliente(_usuarioId), Rotas.Postagem.PorId(Guid.NewGuid()), new EditarPostagemDTO("x", null, null));

        await resposta.DeveSerErroAsync(HttpStatusCode.NotFound, "Postagem não encontrada.");
    }

    [Fact]
    public async Task Editar_QuandoUsuarioNaoPossuiPerfil_DeveRetornar404()
    {
        var postagem = PostagemBuilder.Nova().Build();
        await SemearAsync(postagem);

        var resposta = await PutAsync(CriarCliente(_usuarioId), Rotas.Postagem.PorId(postagem.Id), new EditarPostagemDTO("x", null, null));

        await resposta.DeveSerErroAsync(HttpStatusCode.NotFound, "Perfil não encontrado.");
    }

    [Fact]
    public async Task Editar_QuandoNaoEhAutor_DeveRetornar403ENaoPersistirAlteracoes()
    {
        var postagem = PostagemBuilder.Nova().Build();
        await SemearAsync(postagem, PerfilBuilder.Investidor().ComUsuarioId(_usuarioId).Build());

        var resposta = await PutAsync(CriarCliente(_usuarioId), Rotas.Postagem.PorId(postagem.Id), new EditarPostagemDTO("Invasão.", null, null));

        await resposta.DeveSerErroAsync(HttpStatusCode.Forbidden, "Apenas o autor pode realizar esta ação na postagem.");
        (await ObterPostagemAsync(postagem.Id)).Descricao.ShouldBe(postagem.Descricao);
    }

    [Fact]
    public async Task Editar_QuandoPerfilDesativado_DeveRetornar422()
    {
        var postagem = await SemearPostagemAsync(autorDesativado: true);

        var resposta = await PutAsync(CriarCliente(_usuarioId), Rotas.Postagem.PorId(postagem.Id), new EditarPostagemDTO("x", null, null));

        await resposta.DeveSerErroAsync(HttpStatusCode.UnprocessableEntity, "Não é possível realizar esta ação porque o perfil está desativado.");
        (await ObterPostagemAsync(postagem.Id)).Descricao.ShouldBe(postagem.Descricao);
    }

    [Fact]
    public async Task Editar_QuandoPostagemDesativada_DeveRetornar422()
    {
        var postagem = await SemearPostagemAsync(postagemDesativada: true);

        var resposta = await PutAsync(CriarCliente(_usuarioId), Rotas.Postagem.PorId(postagem.Id), new EditarPostagemDTO("x", null, null));

        await resposta.DeveSerErroAsync(HttpStatusCode.UnprocessableEntity, "Não é possível realizar esta ação porque a postagem está desativada.");
        (await ObterPostagemAsync(postagem.Id)).Descricao.ShouldBe(postagem.Descricao);
    }

    [Theory]
    [InlineData("descricao-vazia", "Descricao", CodigosErroConst.CampoObrigatorio)]
    [InlineData("descricao-longa", "Descricao", CodigosErroConst.TamanhoMaximo)]
    [InlineData("midia-removida-vazia", "MidiasRemovidas[0]", CodigosErroConst.CampoObrigatorio)]
    [InlineData("midia-adicionada-invalida", "MidiasAdicionadas[0].Url", CodigosErroConst.UrlInvalida)]
    [InlineData("midia-adicionada-tipo-invalido", "MidiasAdicionadas[0].Tipo", CodigosErroConst.ValorInvalidoEnum)]
    public async Task Editar_ComPayloadInvalido_DeveRetornar400(string cenario, string campo, string codigo)
    {
        var postagem = await SemearPostagemAsync(midias: 1);

        var resposta = await PutAsync(CriarCliente(_usuarioId), Rotas.Postagem.PorId(postagem.Id), PayloadInvalido(cenario));

        await resposta.DeveSerErroDeValidacaoAsync(campo, codigo);
        (await ObterPostagemAsync(postagem.Id)).DataAtualizacao.ShouldBeNull();
    }

    [Fact]
    public async Task Editar_ComPostagemIdVazio_DeveRetornar400()
    {
        var resposta = await PutAsync(CriarCliente(_usuarioId), Rotas.Postagem.PorId(Guid.Empty), new EditarPostagemDTO("x", null, null));

        await resposta.DeveSerErroDeValidacaoAsync("PostagemId", CodigosErroConst.CampoObrigatorio);
    }

    private async Task<PostagemEntity> SemearPostagemAsync(int midias = 0, bool autorDesativado = false, bool postagemDesativada = false)
    {
        var autorBuilder = PerfilBuilder.Startup().ComUsuarioId(_usuarioId);
        if (autorDesativado)
            autorBuilder.Desativado();

        var postagemBuilder = PostagemBuilder.Nova().ComAutor(autorBuilder.Build()).ComMidias(midias);
        if (postagemDesativada)
            postagemBuilder.Desativada();

        var postagem = postagemBuilder.Build();
        await SemearAsync(postagem);
        return postagem;
    }

    private Task<PostagemEntity> ObterPostagemAsync(Guid id) =>
        ConsultarAsync(db => db.Postagem.Include(p => p.MidiasPostagem).SingleAsync(p => p.Id == id, Ct));

    private static EditarPostagemDTO PayloadInvalido(string cenario) => cenario switch
    {
        "descricao-vazia" => new EditarPostagemDTO("", null, null),
        "descricao-longa" => new EditarPostagemDTO(new string('a', TamanhosColunasConst.Postagem.Descricao + 1), null, null),
        "midia-removida-vazia" => new EditarPostagemDTO("x", [Guid.Empty], null),
        "midia-adicionada-invalida" => new EditarPostagemDTO("x", null, [DtoBuilder.Midia() with { Url = "nao-eh-url" }]),
        "midia-adicionada-tipo-invalido" => new EditarPostagemDTO("x", null, [DtoBuilder.Midia("GIF")]),
        _ => throw new ArgumentOutOfRangeException(nameof(cenario), cenario, null)
    };
}
