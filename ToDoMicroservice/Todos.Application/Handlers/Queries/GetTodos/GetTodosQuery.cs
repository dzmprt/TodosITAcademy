using MediatR;
using Todos.Application.Abstractions.Attributes;
using Todos.Application.DTOs;

namespace Todos.Application.Handlers.Queries.GetTodos;

[RequestAuthorize]
public class GetTodosQuery : ListTodoFilter, IBasePaginationFilter, IRequest<BaseListDto<DTOs.GetTodoDto>>
{
    public int? Limit { get; init; }
    
    public int? Offset { get; init; }
}