using Instartups.Command.Api.DTOs;
using Instartups.Command.Api.DTOs.Postagem;
using Instartups.Command.Application.Common.Response;
using Instartups.Command.Application.Interfaces;
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
}
