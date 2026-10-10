using Instartups.Command.Application.Common.Response;
using Instartups.Command.Domain.Entities;

namespace Instartups.Command.Application.UseCases.EditarPerfilStartup;

public sealed record EditarPerfilStartupResponse(
    PerfilResponse Perfil,
    string Pitch,
    DateOnly DataFundacao,
    int TamanhoEquipe,
    decimal ValorBuscado
)
{
    public static EditarPerfilStartupResponse FromEntity(PerfilEntity perfil)
    {
        return new EditarPerfilStartupResponse(
            PerfilResponse.FromEntity(perfil),
            perfil.Startup!.Pitch,
            perfil.Startup.DataFundacao,
            perfil.Startup.TamanhoEquipe,
            perfil.Startup.ValorBuscado
        );
    }
}
