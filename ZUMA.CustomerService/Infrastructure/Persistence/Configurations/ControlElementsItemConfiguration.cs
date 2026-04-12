using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZUMA.CustomerService.Domain.Entities;

namespace ZUMA.CustomerService.Infrastructure.Persistence.Configurations;

public class ControlElementsItemConfiguration : IEntityTypeConfiguration<ControlsElementsItemEntity>
{
    public void Configure(EntityTypeBuilder<ControlsElementsItemEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Content).IsRequired();
        builder.ToTable("ControlElementsItems");
    }
}
