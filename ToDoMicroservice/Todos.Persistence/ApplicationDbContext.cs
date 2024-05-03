using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Todos.Domain;

namespace Todos.Persistence;

public sealed class ApplicationDbContext : DbContext
{
    #region Owner

    internal DbSet<Owner> Owners { get; } = default!;
    
    internal DbSet<OwnerRole> OwnerRoles { get; } = default!;
    
    #endregion

    #region Todos

    internal DbSet<Todo> Todos { get; } = default!;

    #endregion

    #region Ef

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }

    #endregion
}