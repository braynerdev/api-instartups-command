using System.Security.Claims;
using Instartups.Command.Domain.Exceptions;
using Instartups.Command.Infrastructure.Service;
using Microsoft.AspNetCore.Http;

namespace Instartups.Command.Infrastructure.UnitTests.Service;

public class PegarContextoRequisicaoTests
{
    private static PegarContextoRequisicao CriarComClaims(params Claim[] claims)
    {
        var contexto = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer"))
        };

        return new PegarContextoRequisicao(new HttpContextAccessor { HttpContext = contexto });
    }

    #region UserId

    [Fact]
    public void UserId_QuandoClaimNameIdentifierPresente_DeveRetornarGuidDoUsuario()
    {
        var userId = Guid.NewGuid();
        var sut = CriarComClaims(new Claim(ClaimTypes.NameIdentifier, userId.ToString()));

        sut.UserId.ShouldBe(userId);
    }

    [Fact]
    public void UserId_QuandoClaimNameIdentifierAusente_DeveLancarUsuarioNaoIdentificadoException()
    {
        var sut = CriarComClaims(new Claim("unique_name", "brayner"));

        Should.Throw<UsuarioNaoIdentificadoException>(() => sut.UserId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("nao-eh-guid")]
    [InlineData("12345")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public void UserId_QuandoClaimNameIdentifierNaoEhGuidValido_DeveLancarUsuarioNaoIdentificadoException(string valor)
    {
        var sut = CriarComClaims(new Claim(ClaimTypes.NameIdentifier, valor));

        Should.Throw<UsuarioNaoIdentificadoException>(() => sut.UserId);
    }

    [Fact]
    public void UserId_QuandoHttpContextNulo_DeveLancarUsuarioNaoIdentificadoException()
    {
        var sut = new PegarContextoRequisicao(new HttpContextAccessor { HttpContext = null });

        Should.Throw<UsuarioNaoIdentificadoException>(() => sut.UserId);
    }

    [Fact]
    public void UserId_QuandoUsuarioNaoAutenticado_DeveLancarUsuarioNaoIdentificadoException()
    {
        var sut = new PegarContextoRequisicao(new HttpContextAccessor { HttpContext = new DefaultHttpContext() });

        Should.Throw<UsuarioNaoIdentificadoException>(() => sut.UserId);
    }

    #endregion

    #region Claims opcionais

    [Fact]
    public void ClaimsOpcionais_QuandoPresentes_DeveRetornarValoresDasClaims()
    {
        var sut = CriarComClaims(
            new Claim("unique_name", "brayner"),
            new Claim("Email", "contato@instartups.com"),
            new Claim("PhoneNumber", "+5581999999999"));

        sut.UserName.ShouldBe("brayner");
        sut.Email.ShouldBe("contato@instartups.com");
        sut.PhoneNumber.ShouldBe("+5581999999999");
    }

    [Fact]
    public void ClaimsOpcionais_QuandoAusentes_DeveRetornarNulo()
    {
        var sut = CriarComClaims(new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()));

        sut.UserName.ShouldBeNull();
        sut.Email.ShouldBeNull();
        sut.PhoneNumber.ShouldBeNull();
    }

    [Fact]
    public void ClaimsOpcionais_DeveIgnorarCaixaDoTipoDaClaim()
    {
        var sut = CriarComClaims(
            new Claim("UNIQUE_NAME", "brayner"),
            new Claim("email", "contato@instartups.com"),
            new Claim("phonenumber", "+5581999999999"));

        sut.UserName.ShouldBe("brayner");
        sut.Email.ShouldBe("contato@instartups.com");
        sut.PhoneNumber.ShouldBe("+5581999999999");
    }

    [Fact]
    public void Email_QuandoClaimNoFormatoUriPadrao_DeveRetornarNulo()
    {
        var sut = CriarComClaims(new Claim(ClaimTypes.Email, "contato@instartups.com"));

        sut.Email.ShouldBeNull();
    }

    [Fact]
    public void ClaimsOpcionais_QuandoUsuarioNaoAutenticado_DeveRetornarNulo()
    {
        var sut = new PegarContextoRequisicao(new HttpContextAccessor { HttpContext = new DefaultHttpContext() });

        sut.UserName.ShouldBeNull();
        sut.Email.ShouldBeNull();
        sut.PhoneNumber.ShouldBeNull();
    }

    #endregion
}
