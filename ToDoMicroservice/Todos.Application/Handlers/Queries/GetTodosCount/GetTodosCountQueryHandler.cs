using Todos.Application.Abstractions.Persistence.Repository.Read;
using Todos.Application.Abstractions.Service;
using Todos.Application.BaseRealizations;
using Todos.Application.Caches;
using Todos.Domain;
using Todos.Domain.Enums;

namespace Todos.Application.Handlers.Queries.GetTodosCount;

internal class GetTodosCountQueryHandler : BaseCashedForUserQuery<GetTodosCountQuery, int>
{
    private readonly IBaseReadRepository<Todo> _todos;
    
    private readonly ICurrentUserService _currentUserService;

    public GetTodosCountQueryHandler(IBaseReadRepository<Todo> todos, TodosCountMemoryCache cache, ICurrentUserService currentUserService) : base(cache, currentUserService.CurrentUserId!.Value)
    {
        _todos = todos;
        _currentUserService = currentUserService;
    }
    
    public override async Task<int> SentQueryAsync(GetTodosCountQuery request, CancellationToken cancellationToken)
    {
        return await _todos.AsAsyncRead().CountAsync(_currentUserService.UserInRole(OwnerRolesEnum.Admin) ? ListTodoWhere.WhereForAdmin(request) : ListTodoWhere.WhereForClient(request, _currentUserService.CurrentUserId!.Value), cancellationToken);
    }
}