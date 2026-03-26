using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZUMA.BussinessLogic.Entities.Customer;

namespace ZUMA.BussinessLogic.Infrastructure.Configurations;

public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasKey(u => u.InternalId);

        builder.Property(u => u.PublicId)
            .IsRequired()
            .HasColumnType("uniqueidentifier");

        builder.Property(u => u.Created)
    .IsRequired()
    .HasDefaultValueSql("GETUTCDATE()")
    .HasColumnType("datetime2");

        builder.Property(u => u.Updated)
            .HasColumnType("datetime2");

        builder.Property(u => u.Deleted)
            .HasColumnType("datetime2");

        builder.HasIndex(u => u.Deleted)
    .HasDatabaseName("IX_Users_Deleted");

        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(256)
            .HasColumnType("nvarchar(256)");

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256)
            .HasColumnType("nvarchar(256)");

        builder.Property(u => u.UserName)
            .IsRequired()
            .HasMaxLength(128)
            .HasColumnType("nvarchar(128)");

        builder.Property(u => u.AuthCode)
               .HasMaxLength(12)
               .HasColumnType("nvarchar(12)");

        builder.Property(u => u.AuthCodeExpiration)
        .HasDefaultValueSql("NULL")
        .HasColumnType("datetime2");

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email_Unique");

        builder.ToTable("Users", "dbo");
    }
}