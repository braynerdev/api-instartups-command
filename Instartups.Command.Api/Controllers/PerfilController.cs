using Instartups.Command.Api.DTOs;
using Instartups.Command.Api.DTOs.Perfil.Investidor;
using Instartups.Command.Api.DTOs.Perfil.Startup;
using Instartups.Command.Application.Common.Response;
using Instartups.Command.Application.Interfaces;
using Instartups.Command.Application.UseCases.DeixarDeSeguirPerfil;
using Instartups.Command.Application.UseCases.DesativarPerfil;
using Instartups.Command.Application.UseCases.EditarCoordenadaPerfil;
using Instartups.Command.Application.UseCases.ReativarPerfil;
using Instartups.Command.Application.UseCases.SeguirPerfil;
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
    [ProducesResponseType<ResponseDTO<IdResponse>>(StatusCodes.Status201Created)]
    [ProducesResponseType<ResponseDTO<IEnumerable<ValidationErrorDTO>>>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<ResponseDTO<IdResponse>>> CadastrarPerfilInvestidor([FromBody] CadastrarPerfilInvestidorDTO dto, CancellationToken ct)
    {
        Guid UserId = PegarContextoRequisicao.UserId;
        var command = dto.ToCommand(UserId);

        var response = await MessageBus.InvokeAsync<IdResponse>(command, ct);

        return StatusCode(StatusCodes.Status201Created, ResponseDTO<IdResponse>.Success(MensagensSucessoConst.PerfilCadastrado, response));
    }

    [HttpPut("investidor")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ResponseDTO<IEnumerable<ValidationErrorDTO>>>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> EditarPerfilInvestidor([FromBody] EditarPerfilInvestidorDTO dto, CancellationToken ct)
    {
        Guid UserId = PegarContextoRequisicao.UserId;
        var command = dto.ToCommand(UserId);

        await MessageBus.InvokeAsync(command, ct);

        return NoContent();
    }

    [HttpPost("startup")]
    [Authorize]
    [ProducesResponseType<ResponseDTO<IdResponse>>(StatusCodes.Status201Created)]
    [ProducesResponseType<ResponseDTO<IEnumerable<ValidationErrorDTO>>>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<ResponseDTO<IdResponse>>> CadastrarPerfilStartup([FromBody] CadastrarPerfilStartupDTO dto, CancellationToken ct)
    {
        Guid UserId = PegarContextoRequisicao.UserId;
        var command = dto.ToCommand(UserId);

        var response = await MessageBus.InvokeAsync<IdResponse>(command, ct);

        return StatusCode(StatusCodes.Status201Created, ResponseDTO<IdResponse>.Success(MensagensSucessoConst.PerfilCadastrado, response));
    }

    [HttpPut("startup")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ResponseDTO<IEnumerable<ValidationErrorDTO>>>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> EditarPerfilStartup([FromBody] EditarPerfilStartupDTO dto, CancellationToken ct)
    {
        Guid UserId = PegarContextoRequisicao.UserId;
        var command = dto.ToCommand(UserId);

        await MessageBus.InvokeAsync(command, ct);

        return NoContent();
    }

    [HttpPatch("coordenada")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ResponseDTO<IEnumerable<ValidationErrorDTO>>>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> EditarCoordenadaPerfil([FromBody] CoordenadaDTO dto, CancellationToken ct)
    {
        Guid UserId = PegarContextoRequisicao.UserId;
        var command = new EditarCoordenadaPerfilCommand(dto.ToCommand(), UserId);

        await MessageBus.InvokeAsync(command, ct);

        return NoContent();
    }

    [HttpPatch("desativar")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DesativarPerfil(CancellationToken ct)
    {
        Guid UserId = PegarContextoRequisicao.UserId;
        var command = new DesativarPerfilCommand(UserId);

        await MessageBus.InvokeAsync(command, ct);

        return NoContent();
    }

    [HttpPatch("reativar")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ReativarPerfil(CancellationToken ct)
    {
        Guid UserId = PegarContextoRequisicao.UserId;
        var command = new ReativarPerfilCommand(UserId);

        await MessageBus.InvokeAsync(command, ct);

        return NoContent();
    }

    [HttpPost("{seguidoId:guid}/seguir")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> SeguirPerfil([FromRoute] Guid seguidoId, CancellationToken ct)
    {
        Guid UserId = PegarContextoRequisicao.UserId;
        var command = new SeguirPerfilCommand(seguidoId, UserId);

        await MessageBus.InvokeAsync(command, ct);

        return NoContent();
    }

    [HttpDelete("{seguidoId:guid}/seguir")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeixarDeSeguirPerfil([FromRoute] Guid seguidoId, CancellationToken ct)
    {
        Guid UserId = PegarContextoRequisicao.UserId;
        var command = new DeixarDeSeguirPerfilCommand(seguidoId, UserId);

        await MessageBus.InvokeAsync(command, ct);

        return NoContent();
    }
}
