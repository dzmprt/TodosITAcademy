using FluentValidation;

namespace Todos.Application.Handlers.Commands.UpdateTodo;

internal class UpdateTodoCommandValidator : AbstractValidator<UpdateTodoCommand>
{
    public UpdateTodoCommandValidator()
    {
        RuleFor(e => e.TodoId).GreaterThan(0);
        RuleFor(e => e.Name).NotEmpty().MaximumLength(50);
    }
}