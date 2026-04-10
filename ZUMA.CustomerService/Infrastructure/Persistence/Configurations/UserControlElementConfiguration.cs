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
               .HasForeignKey("ControlElementId")
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.SharedWith)
               .WithOne()
               .HasForeignKey(x => x.UserControlElementId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}