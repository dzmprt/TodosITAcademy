using FluentValidation;

namespace Todos.Application.Handlers.Commands.UpdateTodoIsDone;

internal class UpdateTodoIsDoneCommandValidator : AbstractValidator<UpdateTodoIsDoneCommand>
{
    public UpdateTodoIsDoneCommandValidator()
    {
        RuleFor(e => e.TodoId).GreaterThan(0);
    }
}