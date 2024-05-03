using Todos.Domain.Enums;

namespace Todos.Application.Abstractions.Attributes;

public class RequestAuthorizeAttribute(OwnerRolesEnum[]? roles = null) : Attribute
{
    public OwnerRolesEnum[]? Roles { get; } = roles;
}