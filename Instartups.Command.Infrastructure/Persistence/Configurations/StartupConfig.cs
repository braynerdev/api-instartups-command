using Instartups.Command.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Instartups.Command.Infrastructure.Persistence.Configurations;

public class StartupConfig : BaseConf<StartupEntity>
{
    public override void Configure(EntityTypeBuilder<StartupEntity> builder)
    {
        base.Configure(builder);

        builder.ToTable("startups");

        builder.Property(s => s.Pitch)
            .HasColumnName("pitch")
            .HasColumnType("varchar(1000)")
            .IsRequired();

        builder.Property(s => s.DataFundacao)
            .HasColumnName("data_fundacao")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(s => s.TamanhoEquipe)
            .HasColumnName("tamanho_equipe")
            .HasColumnType("smallint")
            .IsRequired();
        
        builder.Property(s => s.ValorBuscado)
            .HasColumnName("valor_buscado")
            .HasPrecision(12, 2)
            .IsRequired();

        builder.Property(s => s.PerfilId)
            .HasColumnName("perfil_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.HasOne(s => s.Perfil)
            .WithOne(p => p.Startup)
            .HasForeignKey<StartupEntity>(s => s.PerfilId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
