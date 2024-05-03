using AutoMapper;
using MediatR;
using Todos.Application.Abstractions.Persistence.Repository.Writing;
using Todos.Application.Abstractions.Service;
using Todos.Application.Caches;
using Todos.Application.DTOs;
using Todos.Application.Exceptions;
using Todos.Domain;
using Todos.Domain.Enums;

namespace Todos.Application.Handlers.Commands.UpdateTodo;

internal class UpdateTodoCommandHandler : IRequestHandler<UpdateTodoCommand, GetTodoDto>
{
    private readonly IBaseWriteRepository<Todo> _todos;
    
    private readonly IMapper _mapper;
    
    private readonly ICurrentUserService _currentUserService;
    
    private readonly ICleanTodosCacheService _cleanTodosCacheService;

    public UpdateTodoCommandHandler(IBaseWriteRepository<Todo> todos, IMapper mapper, ICurrentUserService currentUserService, ICleanTodosCacheService cleanTodosCacheService)
    {
        _todos = todos;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _cleanTodosCacheService = cleanTodosCacheService;
    }

    public async Task<GetTodoDto> Handle(UpdateTodoCommand request, CancellationToken cancellationToken)
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
        
        todo.UpdateName(request.Name, DateTime.UtcNow);
        todo.UpdateIsDone(request.IsDone, DateTime.UtcNow);
        
        todo = await _todos.UpdateAsync(todo, cancellationToken);
        _cleanTodosCacheService.ClearAllCaches();
        return _mapper.Map<GetTodoDto>(todo);
    }
}