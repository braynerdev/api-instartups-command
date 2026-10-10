using Instartups.Command.Api.DTOs.Perfil.Investidor;
using Instartups.Command.Application.Interfaces;
using Instartups.Command.Application.UseCases.CadastrarPerfilInvestidor;
using Instartups.Command.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Instartups.Command.Api.Controllers;

[Route(RotasBaseConst.Perfil)]
[ApiController]
public class PerfilController(
        IPegarContextoRequisicao PegarContextoRequisicao,
        IMessageBus MessageBus
    ) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<CadastrarPerfilInvestidorResponse>> CadastrarPerfilInvestidor([FromBody] CadastrarPerfilInvestidorDTO dto, CancellationToken ct)
    {
        Guid UserId = PegarContextoRequisicao.UserId;
        var command = dto.ToCommand(UserId);

        var response = await MessageBus.InvokeAsync<CadastrarPerfilInvestidorResponse>(command, ct);

        return StatusCode(StatusCodes.Status201Created, response);
    }
}
