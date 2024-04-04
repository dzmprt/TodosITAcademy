using System.Linq.Expressions;
using AutoFixture;
using AutoMapper;
using Core.Application.Abstractions.Persistence.Repository.Writing;
using Core.Auth.Application.Abstractions.Service;
using Core.Tests;
using Core.Tests.Attributes;
using Core.Tests.Fixtures;
using Core.Tests.Helpers;
using Core.Users.Domain.Enums;
using MediatR;
using Moq;
using Todos.Applications.Caches;
using Todos.Applications.DTOs;
using Todos.Applications.Handlers.Commands.UpdateTodoIsDone;
using Todos.Applications.Handlers.Queries.GetTodos;
using Todos.Domain;
using Xunit.Abstractions;

namespace Todos.UnitTests.Tests.Commands.UpdateTodoIsDone;

public class UpdateTodoIsDoneCommandHandlerTest : RequestHandlerTestBase<UpdateTodoIsDoneCommand, GetTodoDto>
{
    private readonly Mock<IBaseWriteRepository<Todo>> _todosMok = new();

    private readonly Mock<ICurrentUserService> _currentServiceMok = new();

    private readonly ICleanTodosCacheService _cleanTodosCacheService;

    private readonly IMapper _mapper;

    public UpdateTodoIsDoneCommandHandlerTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        _mapper = new AutoMapperFixture(typeof(GetTodosQuery).Assembly).Mapper;
        _cleanTodosCacheService = new CleanTodosCacheService(new Mock<TodoMemoryCache>().Object, new Mock<TodosListMemoryCache>().Object, new Mock<TodosCountMemoryCache>().Object);
    }

    protected override IRequestHandler<UpdateTodoIsDoneCommand, GetTodoDto> CommandHandler =>
        new UpdateTodoIsDoneCommandHandler(_todosMok.Object, _mapper, _currentServiceMok.Object, _cleanTodosCacheService);

    [Theory, FixtureInlineAutoData]
    public async Task Should_BeValid_When_GetTodosByAdmin(UpdateTodoIsDoneCommand command, Guid userId)
    {
        // arrange
        _currentServiceMok.SetupGet(p => p.CurrentUserId).Returns(userId);
        _currentServiceMok.Setup(p => p.UserInRole(ApplicationUserRolesEnum.Admin)).Returns(true);

        var todo = TestFixture.Build<Todo>().Create();
        todo.OwnerId = GuidHelper.GetNotEqualGiud(userId);
        _todosMok.Setup(
            p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
        ).ReturnsAsync(todo);

        // act and assert
        await AssertNotThrow(command);
    }

    [Theory, FixtureInlineAutoData]
    public async Task Should_BeValid_When_GetTodosByClient(UpdateTodoIsDoneCommand command, Guid userId)
    {
        // arrange
        _currentServiceMok.SetupGet(p => p.CurrentUserId).Returns(userId);
        _currentServiceMok.Setup(p => p.UserInRole(ApplicationUserRolesEnum.Client)).Returns(true);

        var todo = TestFixture.Build<Todo>().Create();
        todo.OwnerId = userId;
        _todosMok.Setup(
            p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
        ).ReturnsAsync(todo);

        // act and assert
        await AssertNotThrow(command);
    }

    [Theory, FixtureInlineAutoData]
    public async Task Should_ThrowForbidden_When_GetTodosWithOtherOwnerByClient(UpdateTodoIsDoneCommand command, Guid userId)
    {
        // arrange
        _currentServiceMok.SetupGet(p => p.CurrentUserId).Returns(userId);
        _currentServiceMok.Setup(p => p.UserInRole(ApplicationUserRolesEnum.Client)).Returns(true);

        var todo = TestFixture.Build<Todo>().Create();
        todo.OwnerId = GuidHelper.GetNotEqualGiud(userId);

        _todosMok.Setup(
            p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
        ).ReturnsAsync(todo);

        // act and assert
        await AssertThrowForbiddenFound(command);
    }

    [Theory, FixtureInlineAutoData]
    public async Task Should_ThrowNotFound_When_TodoNotFound(UpdateTodoIsDoneCommand command)
    {
        // arrange

        _todosMok.Setup(
            p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
        ).ReturnsAsync(null as Todo);

        // act and assert
        await AssertThrowNotFound(command);
    }
}
