using FluentValidation;

namespace Todos.Application.Handlers.Commands.DeleteTodo;

internal class DeleteTodoCommandValidator : AbstractValidator<DeleteTodoCommand>
{
    public DeleteTodoCommandValidator()
    {
        RuleFor(e => e.TodoId).GreaterThan(0);
    }
}