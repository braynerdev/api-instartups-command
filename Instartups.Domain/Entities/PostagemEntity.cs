using Instartups.Domain.Entities.Base;
using Instartups.Domain.Entities.ValueObjects;
using System.Collections.ObjectModel;


namespace Instartups.Domain.Entities;

public class PostagemEntity : BaseEntity
{
    public string Descricao { get; private set; } = null!;
    public TotalVO TotalCurtidas { get; private set; } = null!;
    public TotalVO TotalComentarios { get; private set; } = null!;

    public Guid AutorId { get; private set; }
    public PerfilEntity Autor { get; private set; } = null!;
    private readonly List<MidiaPostagem> _midiasPostagem = [];
    public ReadOnlyCollection<MidiaPostagem> MidiasPostagem => _midiasPostagem.AsReadOnly();


}
