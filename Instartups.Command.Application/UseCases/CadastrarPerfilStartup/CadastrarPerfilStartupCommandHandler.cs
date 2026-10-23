using Instartups.Command.Application.Common.Response;
using Instartups.Command.Application.Interfaces;
using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Exceptions;

namespace Instartups.Command.Application.UseCases.CadastrarPerfilStartup;

public class CadastrarPerfilStartupCommandHandler(
        IUnitOfWork unitOfWork
    ) : ICommandHandler<CadastrarPerfilStartupCommand, IdResponse>
{
    public async Task<IdResponse> Handle(CadastrarPerfilStartupCommand command, CancellationToken ct)
    {
        await ValidarUsuarioJaPossuiPerfil(command.Perfil.UserId, ct);

        var perfil = CriarPerfilStartup(command);

        unitOfWork.Perfil.Add(perfil);

        await unitOfWork.CommitAsync(ct);

        return new IdResponse(perfil.Id);
    }

    private async Task ValidarUsuarioJaPossuiPerfil(Guid usuarioId, CancellationToken ct)
    {
        var perfilExistente = await unitOfWork.Perfil.UsuarioJaPossuiPerfilAsync(usuarioId, ct);

        if (perfilExistente)
            throw new UsuarioJaPossuiPerfilException();
    }

    private static PerfilEntity CriarPerfilStartup(CadastrarPerfilStartupCommand command)
    {
        return PerfilEntity.CriarStartup(
                nome: command.Perfil.Nome,
                latitude: command.Perfil.Coordenada.Latitude,
                longitude: command.Perfil.Coordenada.Longitude,
                usuarioId: command.Perfil.UserId,
                imagemPerfilUrl: command.Perfil.ImagemPerfilUrl,
                imagemFundoUrl: command.Perfil.ImagemFundoUrl,
                pitch: command.Pitch,
                dataFundacao: command.DataFundacao,
                tamanhoEquipe: command.TamanhoEquipe,
                valorBuscado: command.ValorBuscado,
            setor: command.Setor
        );
    }
}
