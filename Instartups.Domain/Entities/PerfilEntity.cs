using Instartups.Domain.Entities.Base;
using Instartups.Domain.Entities.ValueObjects;
using Instartups.Domain.Enums;

namespace Instartups.Domain.Entities;

public sealed class PerfilEntity : BaseEntity
{
    public NomeVO Nome { get; private set; } = null!;
    public TiposPerfisEnum TipoPerfil { get; private set; }
    public string? ImagemPerfilUrl { get; private set; }
    public string? ImagemFundoUrl { get; private set; }
    public CoordenadasVO Coordenadas { get; private set; } = null!;
    public TotalVO TotalCurtidas { get; private set; } = null!;
    public TotalVO TotalComentarios { get; private set; } = null!;
    public TotalVO TotalSeguidores { get; private set; } = null!;
    public TotalVO TotalSeguido { get; private set; } = null!;
    public Guid UsuarioId { get; private set; }

    public StartupEntity? Startup { get; private set; }
    public InvestidorEntity? Investidor { get; private set; }


    private PerfilEntity(NomeVO nome, TiposPerfisEnum tipoPerfil, CoordenadasVO coordenadas, Guid usuarioId)
    {
        Nome = nome;
        TipoPerfil = tipoPerfil;
        Coordenadas = coordenadas;
        UsuarioId = usuarioId;
    }



    private static PerfilEntity CriarPerfil(string nome, TiposPerfisEnum tipoPerfil, double latitude, double longitude, Guid usuarioId)
    {
        var coordenadas = CoordenadasVO.Create(latitude, longitude);
        return new PerfilEntity(NomeVO.Create(nome), tipoPerfil, coordenadas, usuarioId);
    }

    private void EditarPerfil(string nome, double latitude, double longitude)
    {
        Nome = NomeVO.Create(nome);
        EditarCoordenadas(latitude, longitude);
    }

    public PerfilEntity EditarCoordenadas(double latitude, double longitude)
    {
        Coordenadas = CoordenadasVO.Create(latitude, longitude);
        return this;
    }



    public static StartupEntity CriarStartup(string nome, TiposPerfisEnum tipoPerfil, double latitude, double longitude, Guid usuarioId, string pitch, DateOnly dataFundacao, int tamanhoEquipe, decimal valorBuscado)
    {
        var perfil = CriarPerfil(nome, tipoPerfil, latitude, longitude, usuarioId);
        return StartupEntity.Criar(pitch, dataFundacao, tamanhoEquipe, valorBuscado, perfil.Id);
    }
    public StartupEntity EditarStartup(string nome, double latitude, double longitude, string pitch, DateOnly dataFundacao, int tamanhoEquipe, decimal valorBuscado)
    {
        EditarPerfil(nome, latitude, longitude);
        return Startup!.Editar(pitch, dataFundacao, tamanhoEquipe, valorBuscado);
    }
    


    public static InvestidorEntity CriarInvestidor(string nome, TiposPerfisEnum tipoPerfil, double latitude, double longitude, Guid usuarioId, string teseInvestimento, decimal ticketMinimo, decimal ticketMaximo)
    {
        var perfil = CriarPerfil(nome, tipoPerfil, latitude, longitude, usuarioId);
        return InvestidorEntity.Criar(teseInvestimento, ticketMinimo, ticketMaximo, perfil.Id);
    }
    public InvestidorEntity EditarInvestidor(string nome, double latitude, double longitude, string teseInvestimento, decimal ticketMinimo, decimal ticketMaximo)
    {
        EditarPerfil(nome, latitude, longitude);
        return Investidor!.Editar(teseInvestimento, ticketMinimo, ticketMinimo);
    }



    public void EditarImagemPerfilUrl(string? imagemPerfilUrl)
    {
        ImagemPerfilUrl = imagemPerfilUrl;
    }

    public void EditarImagemFundoUrl(string? imagemFundoUrl)
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


    public void AdicionarComentario()
    {
        TotalComentarios = TotalComentarios.Adicionar();
    }

    public void RemoverComentario()
    {
        TotalComentarios = TotalComentarios.Remover();
    }


    public void AdicionarSeguido()
    {
        TotalSeguido = TotalSeguido.Adicionar();
    }

    public void RemoverSeguido()
    {
        TotalSeguido = TotalSeguido.Remover();
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
