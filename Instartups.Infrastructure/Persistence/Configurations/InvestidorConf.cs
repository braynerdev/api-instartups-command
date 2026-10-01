using Instartups.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Instartups.Infrastructure.Persistence.Configurations;

public class InvestidorConf : BaseConf<InvestidorEntity>
{
    public override void Configure(EntityTypeBuilder<InvestidorEntity> builder)
    {
        base.Configure(builder);

        builder.ToTable("investidores");

        builder.Property(i => i.TeseInvestimento)
            .HasColumnName("tese_investimento")
            .HasColumnType("varchar(1000)")
            .IsRequired();

        builder.Property(i => i.TicketMinimo)
            .HasColumnName("ticket_minimo")
            .HasPrecision(12, 2)
            .IsRequired();

        builder.Property(i => i.TicketMaximo)
            .HasColumnName("ticket_maximo")
            .HasPrecision(12, 2)
            .IsRequired();

        builder.Property(i => i.PerfilId)
            .HasColumnName("perfil_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.HasOne(i => i.Perfil)
            .WithOne(p => p.Investidor)
            .HasForeignKey<InvestidorEntity>(i => i.PerfilId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
