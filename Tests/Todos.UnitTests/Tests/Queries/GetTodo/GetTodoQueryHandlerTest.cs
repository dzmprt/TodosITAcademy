using System.Linq.Expressions;
using AutoFixture;
using AutoMapper;
using Core.Application.Abstractions.Persistence.Repository.Read;
using Core.Auth.Application.Abstractions.Service;
using Core.Tests;
using Core.Tests.Fixtures;
using Core.Tests.Helpers;
using Core.Users.Domain.Enums;
using MediatR;
using Moq;
using Todos.Applications.Caches;
using Todos.Applications.DTOs;
using Todos.Applications.Handlers.Queries.GetTodo;
using Todos.Domain;
using Xunit.Abstractions;

namespace Todos.UnitTests.Tests.Queries.GetTodo;

public class GetTodoQueryHandlerTest : RequestHandlerTestBase<GetTodoQuery, GetTodoDto>
{
    private readonly Mock<IBaseReadRepository<Todo>> _todosMok = new();

    private readonly Mock<ICurrentUserService> _currentServiceMok = new();

    private readonly Mock<TodoMemoryCache> _mockTodoMemoryCache = new();

    private readonly IMapper _mapper;

    public GetTodoQueryHandlerTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        _mapper = new AutoMapperFixture(typeof(GetTodoQuery).Assembly).Mapper;
    }

    protected override IRequestHandler<GetTodoQuery, GetTodoDto> CommandHandler =>
        new GetTodoQueryHandler(_todosMok.Object, _currentServiceMok.Object, _mapper, _mockTodoMemoryCache.Object);


    [Fact]
    public async Task Should_BeValid_When_GetTodoByAdmin()
    {
        // arrange
        var userId = Guid.NewGuid();
        _currentServiceMok.SetupGet(p => p.CurrentUserId).Returns(userId);
        _currentServiceMok.Setup(p => p.UserInRole(ApplicationUserRolesEnum.Admin)).Returns(true);

        var query = new GetTodoQuery();

        var todo = TestFixture.Build<Todo>().Create();
        todo.OwnerId = GuidHelper.GetNotEqualGiud(userId);
        _todosMok.Setup(
            p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
        ).ReturnsAsync(todo);

        // act and assert
        await AssertNotThrow(query);
    }

    [Fact]
    public async Task Should_BeValid_When_GetTodoByClient()
    {
        // arrange
        var userId = Guid.NewGuid();
        _currentServiceMok.SetupGet(p => p.CurrentUserId).Returns(userId);
        _currentServiceMok.Setup(p => p.UserInRole(ApplicationUserRolesEnum.Admin)).Returns(false);

        var query = new GetTodoQuery();

        var todo = TestFixture.Build<Todo>().Create();
        todo.OwnerId = userId;
        _todosMok.Setup(
            p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
        ).ReturnsAsync(todo);

        // act and assert
        await AssertNotThrow(query);
    }


    [Fact]
    public async Task Should_BeInvalid_When_GetTodoByClient()
    {
        // arrange
        var userId = Guid.NewGuid();
        _currentServiceMok.SetupGet(p => p.CurrentUserId).Returns(userId);
        _currentServiceMok.Setup(p => p.UserInRole(ApplicationUserRolesEnum.Admin)).Returns(false);

        var query = new GetTodoQuery();

        var todo = TestFixture.Build<Todo>().Create();
        todo.OwnerId = GuidHelper.GetNotEqualGiud(userId);
        _todosMok.Setup(
            p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
        ).ReturnsAsync(todo);

        // act and assert
        await AssertThrowForbiddenFound(query);
    }


    [Fact]
    public async Task Should_BeInvalid_When_GetTodoNotFound()
    {
        // arrange
        var userId = Guid.NewGuid();
        _currentServiceMok.SetupGet(p => p.CurrentUserId).Returns(userId);
        _currentServiceMok.Setup(p => p.UserInRole(ApplicationUserRolesEnum.Admin)).Returns(false);

        var query = new GetTodoQuery();

        _todosMok.Setup(
            p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
        ).ReturnsAsync(null as Todo);

        // act and assert
        await AssertThrowNotFound(query);
    }
}