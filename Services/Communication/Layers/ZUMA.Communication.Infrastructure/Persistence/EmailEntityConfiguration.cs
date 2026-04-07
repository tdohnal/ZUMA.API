using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZUMA.Communication.Domain.Entities;

namespace ZUMA.Communication.Infrastructure.Persistence;

public class EmailEntityConfiguration : IEntityTypeConfiguration<EmailEntity>
{
    public void Configure(EntityTypeBuilder<EmailEntity> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.PublicId)
            .IsRequired()
            .HasColumnType("uuid");

        builder.Property(u => u.Created)
    .IsRequired()
    .HasDefaultValueSql("now()")
    .HasColumnType("timestamp with time zone");

        builder.Property(u => u.EmailTemplateType)
   .IsRequired()
   .HasColumnType("int");

        builder.Property(u => u.Recipient)
.IsRequired()
.HasColumnType("varchar(200)");

        builder.Property(u => u.Updated)
            .HasColumnType("timestamp with time zone");

        builder.Property(u => u.Deleted)
            .HasColumnType("timestamp with time zone");

        builder.HasIndex(u => u.Deleted)
    .HasDatabaseName("IX_Emails_Deleted");

        builder.ToTable("Emails", "public");
    }
}