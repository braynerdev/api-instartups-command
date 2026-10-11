using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Exceptions;

namespace Instartups.Command.Domain.Services;

public static class SeguirPerfilService
{
    public static PerfilSeguidorEntity Seguir(PerfilEntity seguidor, PerfilEntity seguido, bool jaSegue)
    {
        if (jaSegue)
            throw new PerfilJaSeguidoException();
        
        ValidarPerfil(seguidor, seguido);

        var perfilSeguidor = PerfilSeguidorEntity.Criar(seguidor, seguido);
        seguidor.AdicionarSeguindo();
        seguido.AdicionarSeguidor();
        return perfilSeguidor;
    }

    public static void DeixarDeSeguir(PerfilEntity seguidor, PerfilEntity seguido)
    {
        ValidarPerfil(seguidor, seguido);

        seguidor.RemoverSeguindo();
        seguido.RemoverSeguidor();
    }

    private static void ValidarPerfil(PerfilEntity seguidor, PerfilEntity seguido)
    {
        if(!seguidor.Ativo || !seguido.Ativo)
            throw new PerfilDesativadoException();
    }
}
