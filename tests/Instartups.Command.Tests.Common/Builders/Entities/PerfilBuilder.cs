using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Enums;
using Instartups.Command.Domain.ValueObjects;
using Instartups.Command.Tests.Common.Extensions;

namespace Instartups.Command.Tests.Common.Builders.Entities;

public sealed class PerfilBuilder
{
    private TiposPerfisEnum _tipoPerfil = TiposPerfisEnum.STARTUP;
    private string _nome = "Instartups Tech";
    private double _latitude = -23.5505;
    private double _longitude = -46.6333;
    private Guid _usuarioId = Guid.NewGuid();
    private string? _imagemPerfilUrl = "https://cdn.instartups.com/perfil.png";
    private string? _imagemFundoUrl = "https://cdn.instartups.com/fundo.png";
    private bool _ativo = true;
    private long _totalCurtidas;
    private long _totalSeguidores;
    private long _totalSeguindo;

    private string _pitch = "Plataforma que conecta startups a investidores.";
    private DateOnly _dataFundacao = new(2020, 1, 15);
    private int _tamanhoEquipe = 10;
    private decimal _valorBuscado = 500_000m;

    private string _teseInvestimento = "Investimento em startups early-stage de tecnologia.";
    private decimal _ticketMinimo = 50_000m;
    private decimal _ticketMaximo = 500_000m;

    public static PerfilBuilder Startup() => new() { _tipoPerfil = TiposPerfisEnum.STARTUP };
    public static PerfilBuilder Investidor() => new() { _tipoPerfil = TiposPerfisEnum.INVESTIDOR };

    public PerfilBuilder ComNome(string nome)
    {
        _nome = nome;
        return this;
    }

    public PerfilBuilder ComCoordenada(double latitude, double longitude)
    {
        _latitude = latitude;
        _longitude = longitude;
        return this;
    }

    public PerfilBuilder ComUsuarioId(Guid usuarioId)
    {
        _usuarioId = usuarioId;
        return this;
    }

    public PerfilBuilder ComImagens(string? imagemPerfilUrl, string? imagemFundoUrl)
    {
        _imagemPerfilUrl = imagemPerfilUrl;
        _imagemFundoUrl = imagemFundoUrl;
        return this;
    }

    public PerfilBuilder ComTotalCurtidas(long total)
    {
        _totalCurtidas = total;
        return this;
    }

    public PerfilBuilder ComTotalSeguidores(long total)
    {
        _totalSeguidores = total;
        return this;
    }

    public PerfilBuilder ComTotalSeguindo(long total)
    {
        _totalSeguindo = total;
        return this;
    }

    public PerfilBuilder Desativado()
    {
        _ativo = false;
        return this;
    }

    public PerfilEntity Build()
    {
        var perfil = _tipoPerfil == TiposPerfisEnum.INVESTIDOR
            ? PerfilEntity.CriarInvestidor(_nome, _latitude, _longitude, _usuarioId, _imagemPerfilUrl, _imagemFundoUrl, _teseInvestimento, _ticketMinimo, _ticketMaximo)
            : PerfilEntity.CriarStartup(_nome, _latitude, _longitude, _usuarioId, _imagemPerfilUrl, _imagemFundoUrl, _pitch, _dataFundacao, _tamanhoEquipe, _valorBuscado);

        perfil
            .DefinirPropriedadePrivada(nameof(PerfilEntity.TotalCurtidas), TotalVO.Create(_totalCurtidas))
            .DefinirPropriedadePrivada(nameof(PerfilEntity.TotalSeguidores), TotalVO.Create(_totalSeguidores))
            .DefinirPropriedadePrivada(nameof(PerfilEntity.TotalSeguindo), TotalVO.Create(_totalSeguindo));

        if (!_ativo)
            perfil.Desativar();

        return perfil;
    }
}
