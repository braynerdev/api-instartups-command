using Instartups.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Instartups.Domain.Entities;

public sealed class CurtidaPostagemEntity : BaseEntity
{
    public Guid PostagemId { get; private set; }
    public PostagemEntity Postagem { get; private set; } = null!;
    public Guid PerfilAutorCurtidaId { get; private set; }
    public PerfilEntity PerfilAutorCurtida { get; private set; } = null!;

    private CurtidaPostagemEntity() { }
    private CurtidaPostagemEntity(Guid postagemId, Guid perfilAutorCurtidaId)
        : base()
    {
        PostagemId = postagemId;
        PerfilAutorCurtidaId = perfilAutorCurtidaId;
    }

    public static CurtidaPostagemEntity Criar(Guid postagemId, Guid perfilAutorCurtidaId)
    {
        var curtida = new CurtidaPostagemEntity(postagemId, perfilAutorCurtidaId);
        return curtida;
    }

}
