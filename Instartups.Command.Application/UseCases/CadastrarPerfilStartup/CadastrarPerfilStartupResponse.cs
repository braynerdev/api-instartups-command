using Instartups.Command.Application.Common.Response;
using Instartups.Command.Domain.Entities;

namespace Instartups.Command.Application.UseCases.CadastrarPerfilStartup;

public sealed record CadastrarPerfilStartupResponse(
    PerfilResponse Perfil,
    string Pitch,
    DateOnly DataFundacao,
    int TamanhoEquipe,
    decimal ValorBuscado
)
{
    public static CadastrarPerfilStartupResponse FromEntity(PerfilEntity perfil)
    {
        return new CadastrarPerfilStartupResponse(
            PerfilResponse.FromEntity(perfil),
            perfil.Startup!.Pitch,
            perfil.Startup.DataFundacao,
            perfil.Startup.TamanhoEquipe,
            perfil.Startup.ValorBuscado
        );
    }
}
