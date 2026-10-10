using Instartups.Command.Domain.Constants;
using Instartups.Command.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Instartups.Command.Infrastructure.Persistence.Configurations;

public class InvestidorConf : BaseConf<InvestidorEntity>
{
    public override void Configure(EntityTypeBuilder<InvestidorEntity> builder)
    {
        base.Configure(builder);

        builder.ToTable("investidores");

        builder.Property(i => i.TeseInvestimento)
            .HasColumnName("tese_investimento")
            .HasColumnType($"varchar({TamanhosColunasConst.Investidor.Tese})")
            .IsRequired();

        builder.Property(i => i.TicketMinimo)
            .HasColumnName("ticket_minimo")
            .HasPrecision(TamanhosColunasConst.Investidor.TicketPrecisao, TamanhosColunasConst.Investidor.TicketEscala)
            .IsRequired();

        builder.Property(i => i.TicketMaximo)
            .HasColumnName("ticket_maximo")
            .HasPrecision(TamanhosColunasConst.Investidor.TicketPrecisao, TamanhosColunasConst.Investidor.TicketEscala)
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
