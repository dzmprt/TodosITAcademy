using FluentValidation;

namespace Todos.Application.Handlers.Commands.CreateTodo;

internal class CreateTodoCommandValidator : AbstractValidator<CreateTodoCommand>
{
    public CreateTodoCommandValidator()
    {
        RuleFor(e => e.Name).NotEmpty().MaximumLength(50);
    }
}