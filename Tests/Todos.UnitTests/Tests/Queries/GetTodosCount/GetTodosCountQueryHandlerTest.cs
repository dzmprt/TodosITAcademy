using System.Linq.Expressions;
using AutoFixture;
using AutoMapper;
using Core.Application.Abstractions.Persistence.Repository.Read;
using Core.Application.DTOs;
using Core.Auth.Application.Abstractions.Service;
using Core.Tests;
using Core.Tests.Fixtures;
using Core.Users.Domain.Enums;
using MediatR;
using Moq;
using Todos.Applications.Caches;
using Todos.Applications.DTOs;
using Todos.Applications.Handlers.Queries.GetTodos;
using Todos.Applications.Handlers.Queries.GetTodosCount;
using Todos.Domain;
using Xunit.Abstractions;

namespace Todos.UnitTests.Tests.Queries.GetTodosCount;

public class GetTodosCountQueryHandlerTest : RequestHandlerTestBase<GetTodosCountQuery, int>
{
    private readonly Mock<IBaseReadRepository<Todo>> _todosMok = new();

    private readonly Mock<ICurrentUserService> _currentServiceMok = new();

    private readonly Mock<TodosCountMemoryCache> _mockTodosCountMemoryCache = new();

    public GetTodosCountQueryHandlerTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    protected override IRequestHandler<GetTodosCountQuery, int> CommandHandler =>
        new GetTodosCountQueryHandler(_todosMok.Object, _mockTodosCountMemoryCache.Object, _currentServiceMok.Object);


    [Fact]
    public async Task Should_BeValid()
    {
        // arrange
        var userId = Guid.NewGuid();
        _currentServiceMok.SetupGet(p => p.CurrentUserId).Returns(userId);

        var query = new GetTodosCountQuery();

        var todos = TestFixture.Build<Todo>().CreateMany(10).ToArray();
        var count = todos.Length;

        _currentServiceMok.Setup(
                p => p.UserInRole(ApplicationUserRolesEnum.Admin))
            .Returns(true);

        _todosMok.Setup(
            p => p.AsAsyncRead().CountAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
        ).ReturnsAsync(count);

        // act and assert
        await AssertNotThrow(query);
    }
}