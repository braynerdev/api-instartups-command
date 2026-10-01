using Instartups.Domain.Entities;
using Instartups.Domain.Enums;
using Instartups.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NpgsqlTypes;

namespace Instartups.Infrastructure.Persistence.Configurations;

public class PerfilConfig : BaseConf<PerfilEntity>
{
    public override void Configure(EntityTypeBuilder<PerfilEntity> builder)
    {
        base.Configure(builder);

        builder.ToTable("perfis");

        builder.Property(p => p.Nome)
            .HasConversion(
                nome => nome.Nome,
                valor => NomeVO.Create(valor)
            )
            .HasColumnName("nome")
            .HasColumnType("varchar(100)")
            .IsRequired();

        builder.Property(p => p.TipoPerfil)
            .HasConversion<int>(
                tipo => (int)tipo,
                valor => (TiposPerfisEnum)valor
            )
            .HasColumnName("tipo_perfil")
            .HasColumnType("smallint")
            .IsRequired();

        builder.Property(p => p.ImagemPerfilUrl)
            .HasColumnName("imagem_perfil_url")
            .HasColumnType("varchar(400)")
            .IsRequired(false);

        builder.Property(p => p.ImagemFundoUrl)
            .HasColumnName("imagem_fundo_url")
            .HasColumnType("varchar(400)")
            .IsRequired(false);

        builder.Property(p => p.Coordenada)
            .HasConversion(
                coordenada => new NpgsqlPoint(coordenada.Longitude, coordenada.Latitude),
                ponto => CoordenadaVO.Create(ponto.Y, ponto.X)
            )
            .HasColumnName("coordenada")
            .HasColumnType("varchar(50)")
            .IsRequired();

        builder.Property(p => p.TotalCurtidas)
            .HasConversion(
                total => total.Total,
                valor => TotalVO.Create(valor)
            )
            .HasColumnName("total_curtidas")
            .HasColumnType("integer")
            .IsRequired();

        builder.Property(p => p.TotalSeguidores)
            .HasConversion(
                total => total.Total,
                valor => TotalVO.Create(valor)
            )
            .HasColumnName("total_seguidores")
            .HasColumnType("integer")
            .IsRequired();

        builder.Property(p => p.TotalSeguindo)
            .HasConversion(
                total => total.Total,
                valor => TotalVO.Create(valor)
            )
            .HasColumnName("total_seguindo")
            .HasColumnType("integer")
            .IsRequired();

        builder.Property(p => p.UsuarioId)
            .HasColumnName("usuario_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.HasOne(p => p.Startup)
            .WithOne(s => s.Perfil)
            .HasForeignKey<StartupEntity>(s => s.PerfilId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Investidor)
            .WithOne(i => i.Perfil)
            .HasForeignKey<InvestidorEntity>(i => i.PerfilId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
