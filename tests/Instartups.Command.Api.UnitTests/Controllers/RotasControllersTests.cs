using System.Reflection;
using Instartups.Command.Api.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Instartups.Command.Api.UnitTests.Controllers;

public class RotasControllersTests
{
    private static readonly Type[] Controllers = [typeof(PerfilController), typeof(PostagemController)];

    public static TheoryData<Type, string, string, string?> Endpoints => new()
    {
        { typeof(PerfilController), nameof(PerfilController.CadastrarPerfilInvestidor), "POST", "investidor" },
        { typeof(PerfilController), nameof(PerfilController.EditarPerfilInvestidor), "PUT", "investidor" },
        { typeof(PerfilController), nameof(PerfilController.CadastrarPerfilStartup), "POST", "startup" },
        { typeof(PerfilController), nameof(PerfilController.EditarPerfilStartup), "PUT", "startup" },
        { typeof(PerfilController), nameof(PerfilController.EditarCoordenadaPerfil), "PATCH", "coordenada" },
        { typeof(PerfilController), nameof(PerfilController.DesativarPerfil), "PATCH", "desativar" },
        { typeof(PerfilController), nameof(PerfilController.ReativarPerfil), "PATCH", "reativar" },
        { typeof(PerfilController), nameof(PerfilController.SeguirPerfil), "POST", "{seguidoId:guid}/seguir" },
        { typeof(PerfilController), nameof(PerfilController.DeixarDeSeguirPerfil), "DELETE", "{seguidoId:guid}/seguir" },
        { typeof(PostagemController), nameof(PostagemController.Post), "POST", null },
        { typeof(PostagemController), nameof(PostagemController.Editar), "PUT", "{postagemId:guid}" },
        { typeof(PostagemController), nameof(PostagemController.Desativar), "PATCH", "{postagemId:guid}/desativar" },
        { typeof(PostagemController), nameof(PostagemController.Reativar), "PATCH", "{postagemId:guid}/reativar" },
        { typeof(PostagemController), nameof(PostagemController.Curtir), "POST", "{postagemId:guid}/curtir" },
        { typeof(PostagemController), nameof(PostagemController.Descurtir), "DELETE", "{postagemId:guid}/curtir" },
    };

    [Theory]
    [InlineData(typeof(PerfilController), "api/instartups/command/perfil")]
    [InlineData(typeof(PostagemController), "api/instartups/command/postagem")]
    public void Controller_DeveTerRotaBaseEApiController(Type controller, string rotaEsperada)
    {
        controller.GetCustomAttribute<RouteAttribute>()!.Template.ShouldBe(rotaEsperada);
        controller.GetCustomAttribute<ApiControllerAttribute>().ShouldNotBeNull();
    }

    [Theory]
    [MemberData(nameof(Endpoints))]
    public void Endpoint_DeveTerVerboERotaEsperados(Type controller, string acao, string verboEsperado, string? rotaEsperada)
    {
        var atributo = controller.GetMethod(acao)!.GetCustomAttribute<HttpMethodAttribute>();

        atributo.ShouldNotBeNull();
        atributo.HttpMethods.ShouldBe([verboEsperado]);
        atributo.Template.ShouldBe(rotaEsperada);
    }

    [Fact]
    public void Endpoints_TodasAsAcoesPublicasDevemExigirAutenticacao()
    {
        var acoes = Controllers.SelectMany(c => c.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)).ToList();

        acoes.ShouldNotBeEmpty();
        acoes.ShouldAllBe(a => a.GetCustomAttribute<AuthorizeAttribute>() != null);
    }

    [Fact]
    public void Endpoints_DevemCobrirTodasAsAcoesPublicas()
    {
        var acoesMapeadas = Endpoints.Select(e => $"{e.Data.Item1.Name}.{e.Data.Item2}").ToHashSet();
        var acoesExistentes = Controllers
            .SelectMany(c => c.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Select(m => $"{c.Name}.{m.Name}"));

        acoesExistentes.ShouldBe(acoesMapeadas, ignoreOrder: true);
    }
}
