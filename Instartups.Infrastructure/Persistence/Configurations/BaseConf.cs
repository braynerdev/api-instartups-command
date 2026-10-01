using Instartups.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Instartups.Infrastructure.Persistence.Configurations;

public class BaseConf<T> : IEntityTypeConfiguration<T> where T : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever();

        builder.Property(x => x.DataCriacao)
            .HasColumnName("data_criacao")
            .HasColumnType("timestamptz")
            .IsRequired();
        builder.HasIndex(x => x.DataCriacao);

        builder.Property(x => x.DataAtualizacao)
            .HasColumnName("data_atualizacao")
            .HasColumnType("timestamptz")
            .IsRequired(false);
        builder.HasIndex(x => x.DataAtualizacao);

        builder.Property(x => x.Ativo)
            .HasColumnName("ativo")
            .HasColumnType("boolean")
            .IsRequired();
    }
}
