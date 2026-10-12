using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Entities.Base;
using Instartups.Command.Tests.Common.Extensions;

namespace Instartups.Command.Domain.UnitTests.Entities;

public class BaseEntityTests
{
    [Fact]
    public void Construtor_DeveGerarIdVersao7NaoVazio()
    {
        var entidade = PostagemEntity.Criar("descrição", Guid.NewGuid());

        entidade.Id.ShouldNotBe(Guid.Empty);
        entidade.Id.Version.ShouldBe(7);
    }

    [Fact]
    public void Construtor_DeveGerarIdsUnicos()
    {
        var primeira = PostagemEntity.Criar("descrição", Guid.NewGuid());
        var segunda = PostagemEntity.Criar("descrição", Guid.NewGuid());

        primeira.Id.ShouldNotBe(segunda.Id);
    }

    [Fact]
    public void Construtor_DeveDefinirDataCriacaoComoAgoraEmUtc()
    {
        var antes = DateTimeOffset.UtcNow;

        var entidade = PostagemEntity.Criar("descrição", Guid.NewGuid());

        entidade.DataCriacao.ShouldBeInRange(antes, DateTimeOffset.UtcNow);
        entidade.DataCriacao.Offset.ShouldBe(TimeSpan.Zero);
    }

    [Fact]
    public void Construtor_DeveCriarAtivaESemDataAtualizacao()
    {
        var entidade = PostagemEntity.Criar("descrição", Guid.NewGuid());

        entidade.Ativo.ShouldBeTrue();
        entidade.DataAtualizacao.ShouldBeNull();
    }

    [Fact]
    public void Equals_QuandoMesmaInstancia_DeveSerIgual()
    {
        var entidade = PostagemEntity.Criar("descrição", Guid.NewGuid());

        entidade.Equals(entidade).ShouldBeTrue();
    }

    [Fact]
    public void Equals_QuandoIdsDiferentes_NaoDeveSerIgual()
    {
        var autorId = Guid.NewGuid();
        var primeira = PostagemEntity.Criar("descrição", autorId);
        var segunda = PostagemEntity.Criar("descrição", autorId);

        primeira.Equals(segunda).ShouldBeFalse();
    }

    [Fact]
    public void Equals_QuandoObjetoNaoEhEntidade_NaoDeveSerIgual()
    {
        var entidade = PostagemEntity.Criar("descrição", Guid.NewGuid());

        entidade.Equals(null).ShouldBeFalse();
        entidade.Equals(entidade.Id).ShouldBeFalse();
    }

    [Fact]
    public void Equals_QuandoEntidadesDeTiposDiferentesComMesmoId_DeveSerIgual()
    {
        var postagem = PostagemEntity.Criar("descrição", Guid.NewGuid());
        var curtida = CurtidaPostagemEntity.Criar(postagem.Id, Guid.NewGuid());
        curtida.DefinirPropriedadePrivada(nameof(BaseEntity.Id), postagem.Id);

        postagem.Equals(curtida).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_DeveSerBaseadoNoId()
    {
        var entidade = PostagemEntity.Criar("descrição", Guid.NewGuid());

        entidade.GetHashCode().ShouldBe(HashCode.Combine(entidade.Id));
    }
}
