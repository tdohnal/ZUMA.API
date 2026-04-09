using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZUMA.CustomerService.Domain.Entities;

namespace ZUMA.CustomerService.Infrastructure.Persistence;

public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.PublicId)
            .IsRequired()
            .HasColumnType("uuid");

        builder.Property(u => u.Created)
    .IsRequired()
    .HasDefaultValueSql("now()")
    .HasColumnType("timestamp with time zone");

        builder.Property(u => u.Updated)
            .HasColumnType("timestamp with time zone");

        builder.Property(u => u.Deleted)
            .HasColumnType("timestamp with time zone");

        builder.HasIndex(u => u.Deleted)
    .HasDatabaseName("IX_Users_Deleted");

        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(256)
            .HasColumnType("varchar(256)");

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256)
            .HasColumnType("varchar(256)");

        builder.Property(u => u.UserName)
            .IsRequired()
            .HasMaxLength(128)
            .HasColumnType("varchar(128)");

        builder.Property(u => u.AuthCode)
               .HasMaxLength(12)
               .HasColumnType("varchar(12)");

        builder.Property(u => u.AuthCodeExpiration)
        .HasDefaultValueSql("NULL")
        .HasColumnType("timestamp with time zone");

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email_Unique");

        builder.OwnsOne(u => u.Address, addressBuilder =>
        {
            addressBuilder.ToJson();

            addressBuilder.Property(a => a.Street).HasMaxLength(200);
            addressBuilder.Property(a => a.City).HasMaxLength(100);
        });

        builder.ToTable("Users", "public");
    }
}