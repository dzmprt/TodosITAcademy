using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todos.Domain;

namespace Todos.Persistence.EntityTypeConfigurations.Owners;

internal class OwnerTypeConfiguration : IEntityTypeConfiguration<Owner>
{
    public void Configure(EntityTypeBuilder<Owner> builder)
    {
        builder.HasKey(e => e.OwnerId);

        builder.HasMany(e => e.Roles);
        
        builder.Navigation(e => e.Roles).AutoInclude();
    }
}