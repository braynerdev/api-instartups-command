using Instartups.Command.Domain.Entities.Base;

namespace Instartups.Command.Domain.Entities;

public sealed class StartupEntity : BaseEntity
{
    public string Pitch { get; private set; } = null!;
    public DateOnly DataFundacao { get; private set; }
    public int TamanhoEquipe { get; private set; }
    public decimal ValorBuscado { get; private set; }
    public Guid PerfilId { get; private set; }
    public PerfilEntity Perfil { get; private set; } = null!;

    private StartupEntity() { }
    private StartupEntity(string pitch, DateOnly dataFundacao, int tamanhoEquipe, decimal valorBuscado, Guid perfilId)
        : base()
    {
        Pitch = pitch;
        DataFundacao = dataFundacao;
        TamanhoEquipe = tamanhoEquipe;
        ValorBuscado = valorBuscado;
        PerfilId = perfilId;
    }

    internal static StartupEntity Criar(string pitch, DateOnly dataFundacao, int tamanhoEquipe, decimal valorBuscado, Guid perfilId)
    {
        return new StartupEntity(pitch, dataFundacao, tamanhoEquipe, valorBuscado, perfilId);
    }

    internal StartupEntity Editar(string pitch, DateOnly dataFundacao, int tamanhoEquipe, decimal valorBuscado)
    {
        Pitch = pitch;
        DataFundacao = dataFundacao;
        TamanhoEquipe = tamanhoEquipe;
        ValorBuscado = valorBuscado;
        Atualizar();
        return this;
    }
}
