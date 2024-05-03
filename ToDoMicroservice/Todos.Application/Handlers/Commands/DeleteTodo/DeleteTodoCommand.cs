using MediatR;
using Todos.Application.Abstractions.Attributes;

namespace Todos.Application.Handlers.Commands.DeleteTodo;

[RequestAuthorize]
public class DeleteTodoCommand : IRequest<Unit>
{
    public int TodoId { get; init; }
}