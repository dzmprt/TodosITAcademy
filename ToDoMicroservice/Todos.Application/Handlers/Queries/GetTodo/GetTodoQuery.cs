using MediatR;
using Todos.Application.Abstractions.Attributes;

namespace Todos.Application.Handlers.Queries.GetTodo;

[RequestAuthorize]
public class GetTodoQuery : IRequest<DTOs.GetTodoDto>
{
    public int TodoId { get; init; }
}