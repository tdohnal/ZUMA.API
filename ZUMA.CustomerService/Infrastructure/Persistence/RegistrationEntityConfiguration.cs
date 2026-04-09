using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZUMA.CustomerService.Domain.Entities;

namespace ZUMA.CustomerService.Infrastructure.Persistence;

public class RegistrationEntityConfiguration : IEntityTypeConfiguration<RegistrationEntity>
{
    public void Configure(EntityTypeBuilder<RegistrationEntity> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.PublicId)
            .IsRequired()
            .HasColumnType("uuid");

        builder.Property(u => u.Created)
            .IsRequired()
            .HasDefaultValueSql("now()") // MS SQL: GETUTCDATE()
            .HasColumnType("timestamp with time zone"); // MS SQL: datetime2

        builder.Property(u => u.ExpirationCodeDate)
               .IsRequired()
               .HasColumnType("timestamp with time zone");

        builder.Property(u => u.ActivationCode)
               .IsRequired()
               .HasMaxLength(10)
               .HasColumnType("varchar(10)"); // MS SQL: nvarchar

        builder.Property(u => u.Updated)
            .HasColumnType("timestamp with time zone");

        builder.Property(u => u.Deleted)
            .HasColumnType("timestamp with time zone");

        builder.HasIndex(u => u.Deleted)
               .HasDatabaseName("IX_Registrations_Deleted");


        // V Postgresu je default "public", "dbo" se nepoužívá
        builder.ToTable("Registrations", "public");
    }
}