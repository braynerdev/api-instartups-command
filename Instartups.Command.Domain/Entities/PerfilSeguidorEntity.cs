using Instartups.Command.Domain.Entities.Base;
using Instartups.Command.Domain.Exceptions;

namespace Instartups.Command.Domain.Entities;

public sealed class PerfilSeguidorEntity : BaseEntity
{
    public Guid SeguidorId { get; private set; }
    public PerfilEntity Seguidor { get; private set; } = null!;
    public Guid SeguidoId { get; private set; }
    public PerfilEntity Seguido { get; private set; } = null!;

    private PerfilSeguidorEntity() { }
    private PerfilSeguidorEntity(PerfilEntity seguidor, PerfilEntity seguido)
        : base()
    {
        Seguidor = seguidor;
        Seguido = seguido;
    }

    public static PerfilSeguidorEntity Criar(PerfilEntity seguidor, PerfilEntity seguido)
    {
        if (!seguido.Ativo)
            throw new PerfilDesativadoException();
        if (!seguidor.Ativo)
            throw new PerfilDesativadoException();

        if (seguidor.Id.Equals(seguido.Id))
            throw new PerfilNaoPodeSeguirASiMesmoException();

        return new(seguidor, seguido);
    }
}
