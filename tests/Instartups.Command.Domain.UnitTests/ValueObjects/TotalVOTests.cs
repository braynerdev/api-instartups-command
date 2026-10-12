using Instartups.Command.Domain.Exceptions;
using Instartups.Command.Domain.ValueObjects;

namespace Instartups.Command.Domain.UnitTests.ValueObjects;

public class TotalVOTests
{
    [Fact]
    public void Zero_DeveRetornarTotalZero()
    {
        var total = TotalVO.Zero();

        total.Total.ShouldBe(0);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(long.MaxValue)]
    public void Create_QuandoValorNaoNegativo_DeveCriarComOValorInformado(long valor)
    {
        var total = TotalVO.Create(valor);

        total.Total.ShouldBe(valor);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(long.MinValue)]
    public void Create_QuandoValorNegativo_DeveLancarTotalNegativoException(long valor)
    {
        var acao = () => TotalVO.Create(valor);

        acao.ShouldThrow<TotalNegativoException>()
            .Message.ShouldBe("O total não pode ser negativo.");
    }

    [Fact]
    public void Adicionar_SemQuantidade_DeveIncrementarEmUm()
    {
        var total = TotalVO.Create(5);

        var resultado = total.Adicionar();

        resultado.Total.ShouldBe(6);
    }

    [Fact]
    public void Adicionar_ComQuantidade_DeveSomarAQuantidade()
    {
        var total = TotalVO.Create(5);

        var resultado = total.Adicionar(10);

        resultado.Total.ShouldBe(15);
    }

    [Fact]
    public void Remover_SemQuantidade_DeveDecrementarEmUm()
    {
        var total = TotalVO.Create(5);

        var resultado = total.Remover();

        resultado.Total.ShouldBe(4);
    }

    [Fact]
    public void Remover_SemQuantidadeQuandoTotalZero_DeveManterZero()
    {
        var total = TotalVO.Zero();

        var resultado = total.Remover();

        resultado.Total.ShouldBe(0);
    }

    [Theory]
    [InlineData(10, 3, 7)]
    [InlineData(10, 10, 0)]
    [InlineData(10, 15, 0)]
    public void Remover_ComQuantidade_DeveSubtrairSemFicarNegativo(long inicial, long quantidade, long esperado)
    {
        var total = TotalVO.Create(inicial);

        var resultado = total.Remover(quantidade);

        resultado.Total.ShouldBe(esperado);
    }

    [Fact]
    public void Adicionar_DeveRetornarNovaInstanciaSemAlterarAOriginal()
    {
        var original = TotalVO.Create(1);

        var resultado = original.Adicionar();

        resultado.ShouldNotBeSameAs(original);
        original.Total.ShouldBe(1);
    }

    [Fact]
    public void Remover_DeveRetornarNovaInstanciaSemAlterarAOriginal()
    {
        var original = TotalVO.Create(1);

        var resultado = original.Remover();

        resultado.ShouldNotBeSameAs(original);
        original.Total.ShouldBe(1);
    }

    [Fact]
    public void Equals_QuandoMesmoTotal_DeveSerIgual()
    {
        TotalVO.Create(3).ShouldBe(TotalVO.Create(3));
    }

    [Fact]
    public void Equals_QuandoTotaisDiferentes_NaoDeveSerIgual()
    {
        TotalVO.Create(3).ShouldNotBe(TotalVO.Create(4));
    }
}
