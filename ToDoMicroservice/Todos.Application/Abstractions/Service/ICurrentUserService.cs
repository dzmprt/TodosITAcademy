using Todos.Domain.Enums;

namespace Todos.Application.Abstractions.Service;

public interface ICurrentUserService
{
    public Guid? CurrentUserId { get; }
    
    public OwnerRolesEnum[] CurrentUserRoles { get; }

    public bool UserInRole(OwnerRolesEnum role);
}