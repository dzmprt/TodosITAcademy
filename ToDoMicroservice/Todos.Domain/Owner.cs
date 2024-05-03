namespace Todos.Domain;

public class Owner
{
    public Guid OwnerId { get; private set; }

    public IReadOnlyCollection<OwnerRole> Roles { get; private set; }
    
    public bool IsActive { get; private set; }

    public Owner(Guid ownerId, OwnerRole[] roles)
    {
        OwnerId = ownerId;
        if ( roles.Any())
        {
            
        }
        Roles = roles;
    }
    
    private Owner(){}
}