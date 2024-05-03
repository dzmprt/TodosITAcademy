using MediatR;
using Todos.Application.Abstractions.Persistence.Repository.Writing;
using Todos.Application.Abstractions.Service;
using Todos.Application.Caches;
using Todos.Application.Exceptions;
using Todos.Domain;
using Todos.Domain.Enums;

namespace Todos.Application.Handlers.Commands.DeleteTodo;

internal class DeleteTodoCommandHandler : IRequestHandler<DeleteTodoCommand, Unit>
{
    private readonly IBaseWriteRepository<Todo> _todos;
    
    private readonly ICurrentUserService _currentUserService;
    
    private readonly ICleanTodosCacheService _cleanTodosCacheService;

    public DeleteTodoCommandHandler(IBaseWriteRepository<Todo> todos, 
        ICurrentUserService currentUserService, ICleanTodosCacheService cleanTodosCacheService)
    {
        _todos = todos;
        _currentUserService = currentUserService;
        _cleanTodosCacheService = cleanTodosCacheService;
    }
    
    public async Task<Unit> Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = await _todos.AsAsyncRead().SingleOrDefaultAsync(e => e.TodoId == request.TodoId, cancellationToken);
        if (todo is null)
        {
            throw new NotFoundException(request);
        }

        if (todo.OwnerId != _currentUserService.CurrentUserId &&
            !_currentUserService.UserInRole(OwnerRolesEnum.Admin))
        {
            throw new ForbiddenException();
        }

        await _todos.RemoveAsync(todo, cancellationToken);
        _cleanTodosCacheService.ClearAllCaches();
        return default;
    }
}