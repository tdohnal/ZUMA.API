using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZUMA.CustomerService.Domain.Entities;

namespace ZUMA.CustomerService.Infrastructure.Persistence.Configurations;

public class ControlElementsAcessConfiguration : IEntityTypeConfiguration<ControlElementsAcessEntity>
{
    public void Configure(EntityTypeBuilder<ControlElementsAcessEntity> builder)
    {
        builder.HasIndex(x => x.UserId);

        builder.HasIndex(x => new { x.UserControlElementId, x.UserId }).IsUnique();
    }
}
