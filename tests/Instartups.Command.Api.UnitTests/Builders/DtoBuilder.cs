using Instartups.Command.Api.DTOs;
using Instartups.Command.Api.DTOs.Perfil;
using Instartups.Command.Api.DTOs.Perfil.Investidor;
using Instartups.Command.Api.DTOs.Perfil.Startup;
using Instartups.Command.Api.DTOs.Postagem;

namespace Instartups.Command.Api.UnitTests.Builders;

public static class DtoBuilder
{
    public static CoordenadaDTO Coordenada() => new(-23.5505, -46.6333);

    public static CadastrarPerfilDTO CadastrarPerfil() => new(
        Nome: "Instartups Tech",
        ImagemPerfilUrl: "https://cdn.instartups.com/perfil.png",
        ImagemFundoUrl: "https://cdn.instartups.com/fundo.png",
        Coordenada: Coordenada()
    );

    public static EditarPerfilDTO EditarPerfil() => new(
        Nome: "Instartups Tech Editada",
        ImagemPerfilUrl: null,
        ImagemFundoUrl: "https://cdn.instartups.com/fundo-novo.png",
        Coordenada: new CoordenadaDTO(-22.9068, -43.1729)
    );

    public static CadastrarPerfilStartupDTO CadastrarStartup() => new(
        Perfil: CadastrarPerfil(),
        Pitch: "Plataforma que conecta startups a investidores.",
        DataFundacao: new DateOnly(2020, 1, 15),
        TamanhoEquipe: 10,
        ValorBuscado: 500_000m
    );

    public static EditarPerfilStartupDTO EditarStartup() => new(
        Perfil: EditarPerfil(),
        Pitch: "Novo pitch.",
        DataFundacao: new DateOnly(2019, 6, 1),
        TamanhoEquipe: 25,
        ValorBuscado: 1_500_000.50m
    );

    public static CadastrarPerfilInvestidorDTO CadastrarInvestidor() => new(
        Perfil: CadastrarPerfil(),
        TeseInvestimento: "Investimento em startups early-stage.",
        TicketMinimo: 50_000m,
        TicketMaximo: 500_000m
    );

    public static EditarPerfilInvestidorDTO EditarInvestidor() => new(
        Perfil: EditarPerfil(),
        TeseInvestimento: "Nova tese.",
        TicketMinimo: 100_000m,
        TicketMaximo: 1_000_000m
    );

    public static MidiaDTO Midia(string tipo = "FOTO", int indice = 0) =>
        new($"https://cdn.instartups.com/midia-{indice}.png", tipo);

    public static CadastrarPostagemDTO CadastrarPostagem() => new(
        Descricao: "Acabamos de fechar nossa rodada seed!",
        Midias: [Midia("FOTO", 0), Midia("video", 1)]
    );

    public static EditarPostagemDTO EditarPostagem() => new(
        Descricao: "Descrição editada.",
        MidiasRemovidas: [Guid.NewGuid(), Guid.NewGuid()],
        MidiasAdicionadas: [Midia("VIDEO", 2)]
    );
}
