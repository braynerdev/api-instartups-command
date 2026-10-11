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
    public async Task<ActionResult<IdResponse>> Post([FromBody] CadastrarPostagemDTO dto)
    {
        var userId = PegarContextoRequisicao.UserId;
        var command = dto.ToCommand(userId);
        
        var response = await MessageBus.InvokeAsync<IdResponse>(command);
        
        return StatusCode(StatusCodes.Status201Created, response);
    }
}
