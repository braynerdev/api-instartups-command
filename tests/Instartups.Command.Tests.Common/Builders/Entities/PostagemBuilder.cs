using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Enums;
using Instartups.Command.Domain.ValueObjects;
using Instartups.Command.Tests.Common.Extensions;

namespace Instartups.Command.Tests.Common.Builders.Entities;

public sealed class PostagemBuilder
{
    private string _descricao = "Acabamos de fechar nossa rodada seed!";
    private PerfilEntity _autor = PerfilBuilder.Startup().Build();
    private int _quantidadeMidias;
    private long _totalCurtidas;
    private bool _ativa = true;

    public static PostagemBuilder Nova() => new();

    public PostagemBuilder ComDescricao(string descricao)
    {
        _descricao = descricao;
        return this;
    }

    public PostagemBuilder ComAutor(PerfilEntity autor)
    {
        _autor = autor;
        return this;
    }

    public PostagemBuilder ComMidias(int quantidade)
    {
        _quantidadeMidias = quantidade;
        return this;
    }

    public PostagemBuilder ComTotalCurtidas(long total)
    {
        _totalCurtidas = total;
        return this;
    }

    public PostagemBuilder Desativada()
    {
        _ativa = false;
        return this;
    }

    public PostagemEntity Build()
    {
        var postagem = PostagemEntity.Criar(_descricao, _autor.Id);

        for (var i = 0; i < _quantidadeMidias; i++)
            postagem.AdicionarMidiaPostagem($"https://cdn.instartups.com/midia-{i}.png", TiposMidiaEnum.FOTO);

        postagem
            .DefinirPropriedadePrivada(nameof(PostagemEntity.Autor), _autor)
            .DefinirPropriedadePrivada(nameof(PostagemEntity.TotalCurtidas), TotalVO.Create(_totalCurtidas));

        if (!_ativa)
            postagem.Desativar();

        return postagem;
    }
}
