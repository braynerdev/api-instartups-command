using Instartups.Domain.Entities.Base;

namespace Instartups.Domain.Entities;

public class PerfilSeguidorEntity : BaseEntity
{
    public Guid SeguidorId { get; private set; }
    public PerfilEntity Seguidor { get; private set; } = null!;
    public Guid SeguidoId { get; private set; }
    public PerfilEntity Seguido { get; private set; } = null!;

    private PerfilSeguidorEntity(Guid seguidorId, Guid seguidoId)
    {
        SeguidorId = seguidorId;
        SeguidoId = seguidoId;
    }

    public static PerfilSeguidorEntity Criar(Guid seguidorId, Guid seguidoId)
    {
        if (seguidorId.Equals(seguidoId))
            throw new Exception("Um perfil não pode seguir a si mesmo."); // ajustar

        return new(seguidorId, seguidoId);
    }
}
