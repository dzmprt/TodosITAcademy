using AutoMapper;
using MediatR;
using Todos.Application.Abstractions.ExternalProviders;
using Todos.Application.Abstractions.Persistence.Repository.Read;
using Todos.Application.Abstractions.Persistence.Repository.Writing;
using Todos.Application.Abstractions.Service;
using Todos.Application.Caches;
using Todos.Application.DTOs;
using Todos.Domain;

namespace Todos.Application.Handlers.Commands.CreateTodo;

internal class CreateTodoCommandHandler : IRequestHandler<CreateTodoCommand, GetTodoDto>
{
    private readonly IBaseWriteRepository<Todo> _todos;
    
    private readonly ICurrentUserService _currentUserService;
    private readonly IOwnersProvider _ownersProvider;

    private readonly IMapper _mapper;
    
    private readonly ICleanTodosCacheService _cleanTodosCacheService;

    public CreateTodoCommandHandler(
        IBaseWriteRepository<Todo> todos, 
        ICurrentUserService currentUserService, 
        IOwnersProvider ownersProvider,
        IMapper mapper, ICleanTodosCacheService cleanTodosCacheService)
    {
        _todos = todos;
        _currentUserService = currentUserService;
        _ownersProvider = ownersProvider;
        _mapper = mapper;
        _cleanTodosCacheService = cleanTodosCacheService;
    }

    public async Task<GetTodoDto> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        var ownerId = _currentUserService.CurrentUserId!.Value;
        var owner = await _ownersProvider.GetOwnerAsync(ownerId, cancellationToken);
        var todo = new Todo(request.Name, owner.OwnerId, DateTime.UtcNow);
        todo = await _todos.AddAsync(todo, cancellationToken);
        
        _cleanTodosCacheService.ClearListCaches();
        
        return _mapper.Map<GetTodoDto>(todo);
    }
}