using Instartups.Command.Application.Common.Command;
using Instartups.Command.Application.UseCases.CadastrarPerfilInvestidor;
using Instartups.Command.Application.UseCases.CadastrarPerfilStartup;
using Instartups.Command.Application.UseCases.EditarCoordenadaPerfil;
using Instartups.Command.Application.UseCases.EditarPerfilInvestidor;
using Instartups.Command.Application.UseCases.EditarPerfilStartup;

namespace Instartups.Command.Tests.Common.Builders.Commands;

public static class PerfilCommandBuilder
{
    public static CoordenadaCommand Coordenada() => new(-23.5505, -46.6333);

    public static CadastrarPerfilCommand CadastrarPerfil(Guid? userId = null) => new(
        Nome: "Instartups Tech",
        ImagemPerfilUrl: "https://cdn.instartups.com/perfil.png",
        ImagemFundoUrl: "https://cdn.instartups.com/fundo.png",
        Coordenada: Coordenada(),
        UserId: userId ?? Guid.NewGuid()
    );

    public static EditarPerfilCommand EditarPerfil(Guid? userId = null) => new(
        Nome: "Instartups Tech Editada",
        ImagemPerfilUrl: "https://cdn.instartups.com/perfil-nova.png",
        ImagemFundoUrl: "https://cdn.instartups.com/fundo-novo.png",
        Coordenada: new CoordenadaCommand(-22.9068, -43.1729),
        UserId: userId ?? Guid.NewGuid()
    );

    public static CadastrarPerfilStartupCommand CadastrarStartup(Guid? userId = null) => new(
        Perfil: CadastrarPerfil(userId),
        Pitch: "Plataforma que conecta startups a investidores.",
        DataFundacao: new DateOnly(2020, 1, 15),
        TamanhoEquipe: 10,
        ValorBuscado: 500_000m
    );

    public static EditarPerfilStartupCommand EditarStartup(Guid? userId = null) => new(
        Perfil: EditarPerfil(userId),
        Pitch: "Novo pitch da startup.",
        DataFundacao: new DateOnly(2019, 6, 1),
        TamanhoEquipe: 25,
        ValorBuscado: 1_500_000.50m
    );

    public static CadastrarPerfilInvestidorCommand CadastrarInvestidor(Guid? userId = null) => new(
        Perfil: CadastrarPerfil(userId),
        TeseInvestimento: "Investimento em startups early-stage de tecnologia.",
        TicketMinimo: 50_000m,
        TicketMaximo: 500_000m
    );

    public static EditarPerfilInvestidorCommand EditarInvestidor(Guid? userId = null) => new(
        Perfil: EditarPerfil(userId),
        TeseInvestimento: "Nova tese: fintechs e healthtechs.",
        TicketMinimo: 100_000m,
        TicketMaximo: 1_000_000m
    );

    public static EditarCoordenadaPerfilCommand EditarCoordenada(Guid? userId = null) => new(
        Coordenada: new CoordenadaCommand(-15.7939, -47.8828),
        UserId: userId ?? Guid.NewGuid()
    );
}
