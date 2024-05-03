using MediatR;
using Todos.Application.Abstractions.Attributes;

namespace Todos.Application.Handlers.Queries.GetTodosCount;

[RequestAuthorize]
public class GetTodosCountQuery : ListTodoFilter, IRequest<int>
{
    
}