using FluentValidation;
using Todos.Application.ValidatorsExtensions;

namespace Todos.Application.Handlers.Queries.GetTodos;

internal class GetTodosQueryValidator : AbstractValidator<GetTodosQuery>
{
    public GetTodosQueryValidator()
    {
        RuleFor(e => e).IsValidPaginationFilter();
        RuleFor(e => e).IsValidListTodoFilter();
    }
}