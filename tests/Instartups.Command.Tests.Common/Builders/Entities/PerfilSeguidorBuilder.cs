using Instartups.Command.Domain.Entities;
using Instartups.Command.Tests.Common.Extensions;

namespace Instartups.Command.Tests.Common.Builders.Entities;

public sealed class PerfilSeguidorBuilder
{
    private PerfilEntity _seguidor = PerfilBuilder.Investidor().Build();
    private PerfilEntity _seguido = PerfilBuilder.Startup().Build();

    public static PerfilSeguidorBuilder Novo() => new();

    public PerfilSeguidorBuilder ComSeguidor(PerfilEntity seguidor)
    {
        _seguidor = seguidor;
        return this;
    }

    public PerfilSeguidorBuilder ComSeguido(PerfilEntity seguido)
    {
        _seguido = seguido;
        return this;
    }

    public PerfilSeguidorEntity Build()
    {
        var perfilSeguidor = PerfilSeguidorEntity.Criar(
            PerfilBuilder.Investidor().Build(),
            PerfilBuilder.Startup().Build());

        return perfilSeguidor
            .DefinirPropriedadePrivada(nameof(PerfilSeguidorEntity.Seguidor), _seguidor)
            .DefinirPropriedadePrivada(nameof(PerfilSeguidorEntity.SeguidorId), _seguidor.Id)
            .DefinirPropriedadePrivada(nameof(PerfilSeguidorEntity.Seguido), _seguido)
            .DefinirPropriedadePrivada(nameof(PerfilSeguidorEntity.SeguidoId), _seguido.Id);
    }
}
