using Instartups.Command.Application.Common.Command;
using Instartups.Command.Domain.Enums;
using Instartups.Command.Domain.Enums.Mapear;

namespace Instartups.Command.Api.DTOs;

public sealed record MidiaDTO(
    string Url,
    string Tipo
)
{
    public MidiaCommand ToCommand()
    {
        return new(
            Url: Url,
            Tipo: Tipo.MapTiposMidia()
        );
    }
}