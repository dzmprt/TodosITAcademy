namespace Todos.Domain;

public class OwnerRole
{
    public int RoleId { get; private set;  }

    public string Name { get; private set;  }
    
    public IReadOnlyCollection<Owner> Owners { get; private set; }

    private OwnerRole(){}
}