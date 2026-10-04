using Instartups.Command.Domain.Entities.Base;
using Instartups.Command.Domain.Enums;
using Instartups.Command.Domain.Exceptions.Base;
using Instartups.Command.Domain.ValueObjects;


namespace Instartups.Command.Domain.Entities;

public sealed class PostagemEntity : BaseEntity
{
    private const int MaxMidias = 5;
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
        Descricao = descricao;
    }

    public void AdicionarMidiaPostagem(string url, TiposMidiaEnum tipoMidia)
    {
        if (_midiasPostagem.Count >= MaxMidias)
            throw new DomainException($"Não é possível adicionar mais de {MaxMidias} mídias a uma postagem.");

        var midiaPostagem = MidiaPostagemEntity.Criar(url, tipoMidia, Id);
        _midiasPostagem.Add(midiaPostagem);
    }

    public void RemoverMidiaPostagem(Guid midiaPostagemId)
    {
        var midiaPostagem = _midiasPostagem.FirstOrDefault(m => m.Id == midiaPostagemId)
            ?? throw new DomainException("Mídia da postagem não encontrada.");

        _midiasPostagem.Remove(midiaPostagem);
    }




    public void AdicionarCurtida()
    {
        TotalCurtidas = TotalCurtidas.Adicionar();
        Autor.AdicionarCurtida();
    }

    public void RemoverCurtida()
    {
        TotalCurtidas = TotalCurtidas.Remover();
        Autor.RemoverCurtida();
    }
}
