using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZUMA.CustomerService.Domain.Entities;

namespace ZUMA.CustomerService.Infrastructure.Persistence.Configurations;

public class ControlElementConfiguration : IEntityTypeConfiguration<ControlElementEntity>
{
    public void Configure(EntityTypeBuilder<ControlElementEntity> builder)
    {
        builder.HasIndex(x => x.PublicId).IsUnique();

        builder.HasMany(x => x.Items)
               .WithOne()
               .HasForeignKey(x => x.ControlElementId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsOne(u => u.ElementsPermission, elementPermissionBuilder =>
        {
            elementPermissionBuilder.ToJson();
            elementPermissionBuilder.OwnsMany(p => p.UserPermissions);
        });

        builder.ToTable("ControlElements");
    }
}