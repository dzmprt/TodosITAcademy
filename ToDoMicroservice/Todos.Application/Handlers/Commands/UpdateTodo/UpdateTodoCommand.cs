using MediatR;
using Todos.Application.Abstractions.Attributes;
using Todos.Application.Abstractions.Mappings;
using Todos.Application.DTOs;
using Todos.Domain;

namespace Todos.Application.Handlers.Commands.UpdateTodo;

[RequestAuthorize]
public class UpdateTodoCommand : IMapTo<Todo>, IRequest<GetTodoDto>
{
    public int TodoId { get; init; }

    public string Name { get; init; } = default!;

    public bool IsDone { get; init; }
}