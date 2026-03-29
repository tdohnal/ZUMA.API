using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZUMA.CustomerService.Entities;

namespace ZUMA.BussinessLogic.Infrastructure.Configurations;

public class RegistrationEntityConfiguration : IEntityTypeConfiguration<RegistrationEntity>
{
    public void Configure(EntityTypeBuilder<RegistrationEntity> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.PublicId)
            .IsRequired()
            .HasColumnType("uniqueidentifier");


        builder.Property(u => u.Created)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()")
            .HasColumnType("datetime2");

        builder.Property(u => u.ExpirationCodeDate)
               .IsRequired()
               .HasColumnType("datetime2");

        builder.Property(u => u.ActivationCode)
       .IsRequired()
       .HasMaxLength(10)
       .HasColumnType("nvarchar(10)");

        builder.Property(u => u.Updated)
            .HasColumnType("datetime2");

        builder.Property(u => u.Deleted)
            .HasColumnType("datetime2");


        builder.HasIndex(u => u.Deleted)
               .HasDatabaseName("IX_Registrations_Deleted");

        builder.ToTable("Registrations", "dbo");
    }
}