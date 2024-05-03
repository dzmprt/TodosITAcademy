using FluentValidation;

namespace Todos.Application.Handlers.Queries.GetTodosCount;

internal class GetTodosCountQueryValidator : AbstractValidator<GetTodosCountQuery>
{
    public GetTodosCountQueryValidator()
    {
        RuleFor(e => e).IsValidListTodoFilter();
    }
}