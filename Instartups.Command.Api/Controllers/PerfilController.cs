using Instartups.Command.Api.DTOs.Perfil.Investidor;
using Instartups.Command.Api.DTOs.Perfil.Startup;
using Instartups.Command.Application.Interfaces;
using Instartups.Command.Application.UseCases.CadastrarPerfilInvestidor;
using Instartups.Command.Application.UseCases.CadastrarPerfilStartup;
using Instartups.Command.Application.UseCases.DesativarPerfil;
using Instartups.Command.Application.UseCases.EditarPerfilInvestidor;
using Instartups.Command.Application.UseCases.EditarPerfilStartup;
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
    [HttpPost("investidor")]
    [Authorize]
    public async Task<ActionResult<CadastrarPerfilInvestidorResponse>> CadastrarPerfilInvestidor([FromBody] CadastrarPerfilInvestidorDTO dto, CancellationToken ct)
    {
        Guid UserId = PegarContextoRequisicao.UserId;
        var command = dto.ToCommand(UserId);

        var response = await MessageBus.InvokeAsync<CadastrarPerfilInvestidorResponse>(command, ct);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("investidor")]
    [Authorize]
    public async Task<ActionResult<EditarPerfilInvestidorResponse>> EditarPerfilInvestidor([FromBody] EditarPerfilInvestidorDTO dto, CancellationToken ct)
    {
        Guid UserId = PegarContextoRequisicao.UserId;
        var command = dto.ToCommand(UserId);

        var response = await MessageBus.InvokeAsync<EditarPerfilInvestidorResponse>(command, ct);

        return Ok(response);
    }

    [HttpPost("startup")]
    [Authorize]
    public async Task<ActionResult<CadastrarPerfilStartupResponse>> CadastrarPerfilStartup([FromBody] CadastrarPerfilStartupDTO dto, CancellationToken ct)
    {
        Guid UserId = PegarContextoRequisicao.UserId;
        var command = dto.ToCommand(UserId);

        var response = await MessageBus.InvokeAsync<CadastrarPerfilStartupResponse>(command, ct);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("startup")]
    [Authorize]
    public async Task<ActionResult<EditarPerfilStartupResponse>> EditarPerfilStartup([FromBody] EditarPerfilStartupDTO dto, CancellationToken ct)
    {
        Guid UserId = PegarContextoRequisicao.UserId;
        var command = dto.ToCommand(UserId);

        var response = await MessageBus.InvokeAsync<EditarPerfilStartupResponse>(command, ct);

        return Ok(response);
    }

    [HttpPatch("desativar")]
    [Authorize]
    public async Task<IActionResult> DesativarPerfil(CancellationToken ct)
    {
        Guid UserId = PegarContextoRequisicao.UserId;
        var command = new DesativarPerfilCommand(UserId);

        await MessageBus.InvokeAsync(command, ct);

        return NoContent();
    }
}
