using Core.Application.Abstractions.Mappings;
using Core.Auth.Application.Attributes;
using MediatR;
using Todos.Domain;

namespace Todos.Applications.Handlers.Queries.GetTodo;

[RequestAuthorize]
public class GetTodoQuery : IRequest<DTOs.GetTodoDto>
{
    public int TodoId { get; init; }
}