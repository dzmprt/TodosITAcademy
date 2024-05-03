using FluentValidation;

namespace Todos.Application.Handlers.Queries.GetTodo;

internal class GetTodoQueryValidator : AbstractValidator<GetTodoQuery>
{
    public GetTodoQueryValidator()
    {
        RuleFor(e => e.TodoId).GreaterThan(0);
    }
}