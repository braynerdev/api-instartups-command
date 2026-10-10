using Instartups.Command.Domain.Entities.Base;
using Instartups.Command.Domain.Enums;
using Instartups.Command.Domain.Exceptions;
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
        if (!Ativo)
            throw new PerfilDesativadoException();

        Nome = NomeVO.Create(nome);
        EditarCoordenadas(latitude, longitude);
        Atualizar();
        return this;
    }

    public PerfilEntity EditarCoordenadas(double latitude, double longitude)
    {
        if (!Ativo)
            throw new PerfilDesativadoException();

        Coordenada = CoordenadaVO.Create(latitude, longitude);
        Atualizar();
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
            throw new PerfilNaoEhStartupException();

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
            throw new PerfilNaoEhInvestidorException();

        var perfil = EditarPerfil(nome, latitude, longitude);
        Investidor!.Editar(teseInvestimento, ticketMinimo, ticketMaximo);
        return perfil;
    }



    public void SetImagemPerfilUrl(string? imagemPerfilUrl)
    {
        ImagemPerfilUrl = imagemPerfilUrl;
        Atualizar();
    }

    public void SetImagemFundoUrl(string? imagemFundoUrl)
    {
        ImagemFundoUrl = imagemFundoUrl;
        Atualizar();
    }



    public void AdicionarCurtida()
    {
        TotalCurtidas = TotalCurtidas.Adicionar();
        Atualizar();
    }
    
    public void RemoverCurtida()
    {
        TotalCurtidas = TotalCurtidas.Remover();
        Atualizar();
    }


    public PerfilSeguidorEntity Seguir(PerfilEntity seguido)
    {
        var perfilSeguidor = PerfilSeguidorEntity.Criar(this, seguido);
        AdicionarSeguindo();
        seguido.AdicionarSeguidor();
        return perfilSeguidor;
    }

    public void DeixarDeSeguir(PerfilEntity seguido)
    {
        if (!Ativo)
            throw new PerfilDesativadoException();

        RemoverSeguindo();
        seguido.RemoverSeguidor();
    }


    public void AdicionarSeguindo()
    {
        TotalSeguindo = TotalSeguindo.Adicionar();
        Atualizar();
    }

    public void RemoverSeguindo()
    {
        TotalSeguindo = TotalSeguindo.Remover();
        Atualizar();
    }


    public void AdicionarSeguidor()
    {
        TotalSeguidores = TotalSeguidores.Adicionar();
        Atualizar();
    }

    public void RemoverSeguidor()
    {
        TotalSeguidores = TotalSeguidores.Remover();
        Atualizar();
    }

}
