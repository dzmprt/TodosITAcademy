using MediatR;
using Todos.Application.DTOs;

namespace Todos.Application.Handlers.Commands.UpdateTodoIsDone;

public class UpdateTodoIsDoneCommand : UpdateTodoIsDonePayload, IRequest<GetTodoDto>
{
    public int TodoId { get; init; }
}