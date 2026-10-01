using Instartups.Domain.Entities;
using Instartups.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Instartups.Infrastructure.Persistence.Configurations;

public class PostagemConfig : BaseConf<PostagemEntity>
{
    public override void Configure(EntityTypeBuilder<PostagemEntity> builder)
    {
        base.Configure(builder);
        builder.ToTable("postagens");

        builder.Property(p => p.Descricao)
            .HasColumnName("descricao")
            .HasColumnType("varchar(500)")
            .IsRequired();

        builder.Property(p => p.TotalCurtidas)
            .HasColumnName("total_curtidas")
            .HasConversion(
                total => total.Total,
                valor => TotalVO.Create(valor))
            .IsRequired();


        builder.Property(p => p.AutorId)
            .HasColumnName("autor_id");

        builder.HasOne(p => p.Autor)
            .WithMany()
            .HasForeignKey(p => p.AutorId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

    }
}
