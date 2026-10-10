using Instartups.Command.Domain.Entities.Base;
using Instartups.Command.Domain.Enums;
using Instartups.Command.Domain.Exceptions;
using Instartups.Command.Domain.Exceptions.Base;
using Instartups.Command.Domain.ValueObjects;

namespace Instartups.Command.Domain.Entities;

public sealed class PerfilEntity : BaseEntity
{
    public NomeVO Nome { get; private set; } = null!;
    public TiposPerfisEnum TipoPerfil { get; private set; }
    public string? ImagemPerfilUrl { get; private set; }
    public string? ImagemFundoUrl { get; private set; }
    public CoordenadaVO Coordenada { get; private set; } = null!;
    public TotalVO TotalCurtidas { get; private set; } = null!;
    public TotalVO TotalSeguidores { get; private set; } = null!;
    public TotalVO TotalSeguindo { get; private set; } = null!;
    public Guid UsuarioId { get; private set; }

    public StartupEntity? Startup { get; private set; }
    public InvestidorEntity? Investidor { get; private set; }


    private PerfilEntity() { } 
    private PerfilEntity(NomeVO nome, TiposPerfisEnum tipoPerfil, CoordenadaVO coordenada, Guid usuarioId)
        : base()
    {
        Nome = nome;
        TipoPerfil = tipoPerfil;
        Coordenada = coordenada;
        UsuarioId = usuarioId;
        TotalCurtidas = TotalVO.Zero();
        TotalSeguidores = TotalVO.Zero();
        TotalSeguindo = TotalVO.Zero();
    }



    private static PerfilEntity CriarPerfil(string nome, TiposPerfisEnum tipoPerfil, double latitude, double longitude, Guid usuarioId)
    {
        var coordenada = CoordenadaVO.Create(latitude, longitude);
        return new PerfilEntity(NomeVO.Create(nome), tipoPerfil, coordenada, usuarioId);
    }

    private PerfilEntity EditarPerfil(string nome, double latitude, double longitude)
    {
        Nome = NomeVO.Create(nome);
        EditarCoordenadas(latitude, longitude);
        return this;
    }

    public PerfilEntity EditarCoordenadas(double latitude, double longitude)
    {
        Coordenada = CoordenadaVO.Create(latitude, longitude);
        return this;
    }



    public static PerfilEntity CriarStartup(string nome, double latitude, double longitude, Guid usuarioId, string pitch, DateOnly dataFundacao, int tamanhoEquipe, decimal valorBuscado)
    {
        var perfil = CriarPerfil(nome, TiposPerfisEnum.STARTUP, latitude, longitude, usuarioId);
        var startup = StartupEntity.Criar(pitch, dataFundacao, tamanhoEquipe, valorBuscado, perfil.Id);
        perfil.Startup = startup;
        return perfil;
    }
    public PerfilEntity EditarStartup(string nome, double latitude, double longitude, string pitch, DateOnly dataFundacao, int tamanhoEquipe, decimal valorBuscado)
    {
        if(TipoPerfil != TiposPerfisEnum.STARTUP)
            throw new DomainException("O perfil não é do tipo STARTUP.");

        if (!Ativo)
            throw new PerfilDesativadoException();

        var perfil = EditarPerfil(nome, latitude, longitude);
        Startup!.Editar(pitch, dataFundacao, tamanhoEquipe, valorBuscado);
        return perfil;
    }
    


    public static PerfilEntity CriarInvestidor(string nome, double latitude, double longitude, Guid usuarioId, string teseInvestimento, decimal ticketMinimo, decimal ticketMaximo)
    {
        var perfil = CriarPerfil(nome, TiposPerfisEnum.INVESTIDOR, latitude, longitude, usuarioId);
        var investidor = InvestidorEntity.Criar(teseInvestimento, ticketMinimo, ticketMaximo, perfil.Id);
        perfil.Investidor = investidor;
        return perfil;
    }
    public PerfilEntity EditarInvestidor(string nome, double latitude, double longitude, string teseInvestimento, decimal ticketMinimo, decimal ticketMaximo)
    {
        if (TipoPerfil != TiposPerfisEnum.INVESTIDOR)
            throw new DomainException("O perfil não é do tipo INVESTIDOR.");

        if (!Ativo)
            throw new PerfilDesativadoException();

        var perfil = EditarPerfil(nome, latitude, longitude);
        Investidor!.Editar(teseInvestimento, ticketMinimo, ticketMaximo);
        return perfil;
    }



    public void SetImagemPerfilUrl(string? imagemPerfilUrl)
    {
        ImagemPerfilUrl = imagemPerfilUrl;
    }

    public void SetImagemFundoUrl(string? imagemFundoUrl)
    {
        ImagemFundoUrl = imagemFundoUrl;
    }



    public void AdicionarCurtida()
    {
        TotalCurtidas = TotalCurtidas.Adicionar();
    }
    
    public void RemoverCurtida()
    {
        TotalCurtidas = TotalCurtidas.Remover();
    }


    public void AdicionarSeguindo()
    {
        TotalSeguindo = TotalSeguindo.Adicionar();
    }

    public void RemoverSeguindo()
    {
        TotalSeguindo = TotalSeguindo.Remover();
    }


    public void AdicionarSeguidor()
    {
        TotalSeguidores = TotalSeguidores.Adicionar();
    }

    public void RemoverSeguidor()
    {
        TotalSeguidores = TotalSeguidores.Remover();
    }

}
