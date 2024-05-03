using MediatR;
using Todos.Application.Abstractions.Attributes;
using Todos.Application.DTOs;

namespace Todos.Application.Handlers.Commands.CreateTodo;

[RequestAuthorize]
public class CreateTodoCommand : IRequest<GetTodoDto>
{
    public string Name { get; init; } = default!;
}