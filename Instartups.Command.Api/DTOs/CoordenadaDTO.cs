using Instartups.Command.Application.Common.Command;

namespace Instartups.Command.Api.DTOs;

public sealed record CoordenadaDTO(
    double Latitude,
    double Longitude
)
{
    public CoordenadaCommand ToCommand()
    {
        return new CoordenadaCommand(Latitude, Longitude);
    }
}
