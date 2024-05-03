namespace Todos.ExternalProviders.Models;

public class GetUserDto
{
    public Guid ApplicationUserId { get; set; }
    
    public int[] Roles { get; set; } = default!;
}