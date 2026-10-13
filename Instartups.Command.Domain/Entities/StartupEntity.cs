using Instartups.Command.Domain.Entities.Base;
using Instartups.Command.Domain.Enums;

namespace Instartups.Command.Domain.Entities;

public sealed class StartupEntity : BaseEntity
{
    public string Pitch { get; private set; } = null!;
    public DateOnly DataFundacao { get; private set; }
    public int TamanhoEquipe { get; private set; }
    public decimal ValorBuscado { get; private set; }
    public SetorStartupEnum Setor { get; private set; }
    public Guid PerfilId { get; private set; }
    public PerfilEntity Perfil { get; private set; } = null!;

    private StartupEntity() { }
    private StartupEntity(string pitch, DateOnly dataFundacao, int tamanhoEquipe, decimal valorBuscado, SetorStartupEnum setor, Guid perfilId)
        : base()
    {
        Pitch = pitch;
        DataFundacao = dataFundacao;
        TamanhoEquipe = tamanhoEquipe;
        ValorBuscado = valorBuscado;
        Setor = setor;
        PerfilId = perfilId;
    }

    internal static StartupEntity Criar(string pitch, DateOnly dataFundacao, int tamanhoEquipe, decimal valorBuscado, SetorStartupEnum setor, Guid perfilId)
    {
        return new StartupEntity(pitch, dataFundacao, tamanhoEquipe, valorBuscado, setor, perfilId);
    }

    internal StartupEntity Editar(string pitch, DateOnly dataFundacao, int tamanhoEquipe, decimal valorBuscado, SetorStartupEnum setor)
    {
        Pitch = pitch;
        DataFundacao = dataFundacao;
        TamanhoEquipe = tamanhoEquipe;
        ValorBuscado = valorBuscado;
        Setor = setor;
        Atualizar();
        return this;
    }
}
