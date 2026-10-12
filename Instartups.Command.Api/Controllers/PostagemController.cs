using Instartups.Command.Api.DTOs;
using Instartups.Command.Api.DTOs.Postagem;
using Instartups.Command.Application.Common.Response;
using Instartups.Command.Application.Interfaces;
using Instartups.Command.Application.UseCases.CurtirPostagem;
using Instartups.Command.Application.UseCases.DescurtirPostagem;
using Instartups.Command.Application.UseCases.DesativarPostagem;
using Instartups.Command.Application.UseCases.ReativarPostagem;
using Instartups.Command.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Instartups.Command.Api.Controllers;

[Route(RotasBaseConst.Postagem)]
[ApiController]
public class PostagemController(
    IPegarContextoRequisicao PegarContextoRequisicao,
    IMessageBus MessageBus
    ) : ControllerBase
{
    [HttpPost]
    [Authorize]
    [ProducesResponseType<ResponseDTO<IdResponse>>(StatusCodes.Status201Created)]
    [ProducesResponseType<ResponseDTO<IEnumerable<ValidationErrorDTO>>>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<ResponseDTO<IdResponse>>> Post([FromBody] CadastrarPostagemDTO dto, CancellationToken ct)
    {
        var userId = PegarContextoRequisicao.UserId;
        var command = dto.ToCommand(userId);

        var response = await MessageBus.InvokeAsync<IdResponse>(command, ct);

        return StatusCode(StatusCodes.Status201Created, ResponseDTO<IdResponse>.Success(MensagensSucessoConst.PostagemCadastrada, response));
    }

    [HttpPut("{postagemId:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ResponseDTO<IEnumerable<ValidationErrorDTO>>>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Editar([FromRoute] Guid postagemId, [FromBody] EditarPostagemDTO dto, CancellationToken ct)
    {
        var userId = PegarContextoRequisicao.UserId;
        var command = dto.ToCommand(postagemId, userId);

        await MessageBus.InvokeAsync(command, ct);

        return NoContent();
    }

    [HttpPatch("{postagemId:guid}/desativar")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ResponseDTO<IEnumerable<ValidationErrorDTO>>>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Desativar([FromRoute] Guid postagemId, CancellationToken ct)
    {
        var userId = PegarContextoRequisicao.UserId;
        var command = new DesativarPostagemCommand(postagemId, userId);

        await MessageBus.InvokeAsync(command, ct);

        return NoContent();
    }

    [HttpPatch("{postagemId:guid}/reativar")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ResponseDTO<IEnumerable<ValidationErrorDTO>>>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Reativar([FromRoute] Guid postagemId, CancellationToken ct)
    {
        var userId = PegarContextoRequisicao.UserId;
        var command = new ReativarPostagemCommand(postagemId, userId);

        await MessageBus.InvokeAsync(command, ct);

        return NoContent();
    }

    [HttpPost("{postagemId:guid}/curtir")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ResponseDTO<IEnumerable<ValidationErrorDTO>>>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Curtir([FromRoute] Guid postagemId, CancellationToken ct)
    {
        var userId = PegarContextoRequisicao.UserId;
        var command = new CurtirPostagemCommand(postagemId, userId);

        await MessageBus.InvokeAsync(command, ct);

        return NoContent();
    }

    [HttpDelete("{postagemId:guid}/curtir")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ResponseDTO<IEnumerable<ValidationErrorDTO>>>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ResponseDTO<string>>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Descurtir([FromRoute] Guid postagemId, CancellationToken ct)
    {
        var userId = PegarContextoRequisicao.UserId;
        var command = new DescurtirPostagemCommand(postagemId, userId);

        await MessageBus.InvokeAsync(command, ct);

        return NoContent();
    }
}