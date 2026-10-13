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
    private PerfilEntity(NomeVO nome, TiposPerfisEnum tipoPerfil, CoordenadaVO coordenada, Guid usuarioId, string? imagemPerfilUrl, string? imagemFundoUrl)
        : base()
    {
        Nome = nome;
        TipoPerfil = tipoPerfil;
        Coordenada = coordenada;
        UsuarioId = usuarioId;
        ImagemPerfilUrl = imagemPerfilUrl;
        ImagemFundoUrl = imagemFundoUrl;
        TotalCurtidas = TotalVO.Zero();
        TotalSeguidores = TotalVO.Zero();
        TotalSeguindo = TotalVO.Zero();
    }



    private static PerfilEntity CriarPerfil(string nome, TiposPerfisEnum tipoPerfil, double latitude, double longitude, Guid usuarioId, string? imagemPerfilUrl, string? imagemFundoUrl)
    {
        var coordenada = CoordenadaVO.Create(latitude, longitude);
        return new PerfilEntity(NomeVO.Create(nome), tipoPerfil, coordenada, usuarioId, imagemPerfilUrl, imagemFundoUrl);
    }
    private PerfilEntity EditarPerfil(string nome, double latitude, double longitude, string? imagemPerfilUrl, string? imagemFundoUrl)
    {
        if (!Ativo)
            throw new PerfilDesativadoException();

        Nome = NomeVO.Create(nome);
        Coordenada = CoordenadaVO.Create(latitude, longitude);
        ImagemPerfilUrl = imagemPerfilUrl;
        ImagemFundoUrl = imagemFundoUrl;
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



    public static PerfilEntity CriarStartup(string nome, double latitude, double longitude, Guid usuarioId, string? imagemPerfilUrl, string? imagemFundoUrl, string pitch, DateOnly dataFundacao, int tamanhoEquipe, decimal valorBuscado, SetorStartupEnum setor)
    {
        var perfil = CriarPerfil(nome, TiposPerfisEnum.Startup, latitude, longitude, usuarioId, imagemPerfilUrl, imagemFundoUrl);
        var startup = StartupEntity.Criar(pitch, dataFundacao, tamanhoEquipe, valorBuscado, setor, perfil.Id);
        perfil.Startup = startup;
        return perfil;
    }
    public PerfilEntity EditarStartup(string nome, double latitude, double longitude, string? imagemPerfilUrl, string? imagemFundoUrl, string pitch, DateOnly dataFundacao, int tamanhoEquipe, decimal valorBuscado, SetorStartupEnum setor)
    {
        if(TipoPerfil != TiposPerfisEnum.Startup)
            throw new PerfilNaoEhStartupException();

        var perfil = EditarPerfil(nome, latitude, longitude, imagemPerfilUrl, imagemFundoUrl);
        Startup!.Editar(pitch, dataFundacao, tamanhoEquipe, valorBuscado, setor);
        return perfil;
    }



    public static PerfilEntity CriarInvestidor(string nome, double latitude, double longitude, Guid usuarioId, string? imagemPerfilUrl, string? imagemFundoUrl, string teseInvestimento, decimal ticketMinimo, decimal ticketMaximo)
    {
        var perfil = CriarPerfil(nome, TiposPerfisEnum.Investidor, latitude, longitude, usuarioId, imagemPerfilUrl, imagemFundoUrl);
        var investidor = InvestidorEntity.Criar(teseInvestimento, ticketMinimo, ticketMaximo, perfil.Id);
        perfil.Investidor = investidor;
        return perfil;
    }
    public PerfilEntity EditarInvestidor(string nome, double latitude, double longitude, string? imagemPerfilUrl, string? imagemFundoUrl, string teseInvestimento, decimal ticketMinimo, decimal ticketMaximo)
    {
        if (TipoPerfil != TiposPerfisEnum.Investidor)
            throw new PerfilNaoEhInvestidorException();

        var perfil = EditarPerfil(nome, latitude, longitude, imagemPerfilUrl, imagemFundoUrl);
        Investidor!.Editar(teseInvestimento, ticketMinimo, ticketMaximo);
        return perfil;
    }



    public override void Ativar()
    {
        if (Ativo)
            throw new PerfilJaAtivoException();

        base.Ativar();
    }
    public override void Desativar()
    {
        if (!Ativo)
            throw new PerfilJaDesativadoException();

        base.Desativar();
    }


    internal void AdicionarCurtida()
    {
        TotalCurtidas = TotalCurtidas.Adicionar();
        Atualizar();
    }
    internal void RemoverCurtida()
    {
        TotalCurtidas = TotalCurtidas.Remover();
        Atualizar();
    }
    internal void AdicionarCurtidas(TotalVO quantidade)
    {
        TotalCurtidas = TotalCurtidas.Adicionar(quantidade.Total);
        Atualizar();
    }
    internal void RemoverCurtidas(TotalVO quantidade)
    {
        TotalCurtidas = TotalCurtidas.Remover(quantidade.Total);
        Atualizar();
    }
    internal void AdicionarSeguindo()
    {
        TotalSeguindo = TotalSeguindo.Adicionar();
        Atualizar();
    }
    internal void RemoverSeguindo()
    {
        TotalSeguindo = TotalSeguindo.Remover();
        Atualizar();
    }
    internal void AdicionarSeguidor()
    {
        TotalSeguidores = TotalSeguidores.Adicionar();
        Atualizar();
    }
    internal void RemoverSeguidor()
    {
        TotalSeguidores = TotalSeguidores.Remover();
        Atualizar();
    }

}
