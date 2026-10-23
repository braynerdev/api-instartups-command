using Instartups.Command.Domain.Entities.Base;
using Instartups.Command.Domain.Enums;
using Instartups.Command.Domain.Exceptions;
using Instartups.Command.Domain.ValueObjects;


namespace Instartups.Command.Domain.Entities;

public sealed class PostagemEntity : BaseEntity
{
    private const int MaxMidias = 10;
    public string Descricao { get; private set; } = null!;
    public TotalVO TotalCurtidas { get; private set; } = null!;
    public Guid AutorId { get; private set; }
    public PerfilEntity Autor { get; private set; } = null!;

    private readonly List<MidiaPostagemEntity> _midiasPostagem = [];
    public IReadOnlyCollection<MidiaPostagemEntity> MidiasPostagem => _midiasPostagem.AsReadOnly();


    private PostagemEntity() { }
    private PostagemEntity(string descricao, Guid autorId)
        : base()
    {
        Descricao = descricao;
        AutorId = autorId;
        TotalCurtidas = TotalVO.Zero();
    }




    public static PostagemEntity Criar(string descricao, Guid autorId)
    {
        return new PostagemEntity(descricao, autorId);
    }

    public void EditarDescricao(string descricao)
    {
        ValidarAtiva();

        Descricao = descricao;
        Atualizar();
    }

    public void AdicionarMidiaPostagem(string url, TiposMidiaEnum tipoMidia)
    {
        ValidarAtiva();

        if (_midiasPostagem.Count >= MaxMidias)
            throw new LimiteMidiasPostagemExcedidoException(MaxMidias);

        var midiaPostagem = MidiaPostagemEntity.Criar(url, tipoMidia, Id);
        _midiasPostagem.Add(midiaPostagem);
    }

    public void RemoverMidiaPostagem(Guid midiaPostagemId)
    {
        ValidarAtiva();

        var midiaPostagem = _midiasPostagem.FirstOrDefault(m => m.Id == midiaPostagemId)
            ?? throw new MidiaPostagemNaoEncontradaException();

        _midiasPostagem.Remove(midiaPostagem);
    }

    public override void Ativar()
    {
        if (Ativo)
            throw new PostagemJaAtivaException();

        base.Ativar();
    }

    public override void Desativar()
    {
        if (!Ativo)
            throw new PostagemJaDesativadaException();

        base.Desativar();
    }

    private void ValidarAtiva()
    {
        if (!Ativo)
            throw new PostagemDesativadaException();
    }

    internal void AdicionarCurtida()
    {
        TotalCurtidas = TotalCurtidas.Adicionar();
    }

    internal void RemoverCurtida()
    {
        TotalCurtidas = TotalCurtidas.Remover();
    }
}
