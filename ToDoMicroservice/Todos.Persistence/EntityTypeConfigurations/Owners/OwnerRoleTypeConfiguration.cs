using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todos.Domain;

namespace Todos.Persistence.EntityTypeConfigurations.Owners;

internal class OwnerRoleTypeConfiguration : IEntityTypeConfiguration<OwnerRole>
{
    public void Configure(EntityTypeBuilder<OwnerRole> builder)
    {
        builder.HasKey(e => e.RoleId);

        builder.Property(e => e.Name).HasMaxLength(20).IsRequired();

        builder.HasMany(e => e.Owners);
    }
}