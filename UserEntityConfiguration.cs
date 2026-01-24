using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZUMA.BussinessLogic.Infrastructure.Entities.Customer;

namespace ZUMA.BussinessLogic.Infrastructure.Configurations;

/// <summary>
/// EF Core konfigurace pro UserEntity.
/// Definuje mapování sloupců, indexy, constraints, atd.
/// </summary>
public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        // Primární klíč
        builder.HasKey(u => u.Id);

        // Sloupce
        builder.Property(u => u.Name)
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

        builder.Property(u => u.Password)
            .IsRequired()
            .HasMaxLength(256)
            .HasColumnType("nvarchar(256)");

        // Audit pole
        builder.Property(u => u.Created)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()")
            .HasColumnType("datetime2");

        builder.Property(u => u.Updated)
            .HasColumnType("datetime2");

        builder.Property(u => u.Deleted)
            .HasColumnType("datetime2");

        // Indexy
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasName("IX_Users_Email_Unique");

        builder.HasIndex(u => u.UserName)
            .IsUnique()
            .HasName("IX_Users_UserName_Unique");

        builder.HasIndex(u => u.Deleted)
            .HasName("IX_Users_Deleted");

        // Tabulka a schéma
        builder.ToTable("Users", "dbo");
    }
}