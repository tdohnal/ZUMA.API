using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZUMA.BussinessLogic.Entities.Customer;

namespace ZUMA.BussinessLogic.Infrastructure.Configurations;

public class EmailEntityConfiguration : IEntityTypeConfiguration<EmailEntity>
{
    public void Configure(EntityTypeBuilder<EmailEntity> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.PublicId)
            .IsRequired()
            .HasColumnType("uniqueidentifier");

        builder.Property(u => u.Created)
    .IsRequired()
    .HasDefaultValueSql("GETUTCDATE()")
    .HasColumnType("datetime2");

        builder.Property(u => u.EmailTemplateType)
   .IsRequired()
   .HasColumnType("int");

        builder.Property(u => u.Recipient)
.IsRequired()
.HasColumnType("nvarchar(200)");

        builder.Property(u => u.Updated)
            .HasColumnType("datetime2");

        builder.Property(u => u.Deleted)
            .HasColumnType("datetime2");

        builder.HasIndex(u => u.Deleted)
    .HasDatabaseName("IX_Emails_Deleted");

        builder.ToTable("Emails", "dbo");
    }
}