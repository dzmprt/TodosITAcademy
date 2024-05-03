using Todos.Domain;

namespace Todos.Application.Abstractions.ExternalProviders;

public interface IOwnersProvider
{ 
    Task<Owner> GetOwnerAsync(Guid ownerId, CancellationToken cancellationToken);
}