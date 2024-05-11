using AutoFixture;
using AutoMapper;
using Core.Application.Abstractions.Persistence.Repository.Writing;
using Core.Auth.Application.Abstractions.Service;
using Core.Tests;
using Core.Tests.Attributes;
using Core.Tests.Fixtures;
using Core.Tests.Helpers;
using MediatR;
using Moq;
using System.Linq.Expressions;
using Todos.Applications.Caches;
using Todos.Applications.Handlers.Queries.GetTodosCount;
using Todos.Domain;
using Xunit.Abstractions;

namespace Todos.UnitTests.Tests.Queries.GetTodosCount;

public class GetTodosCountQueryHandlerTest : RequestHandlerTestBase<GetTodosCountQuery, int>
{
    private readonly Mock<IBaseWriteRepository<Todo>> _todosMok = new();

    private readonly Mock<ICurrentUserService> _currentServiceMok = new();

    private readonly TodosCountMemoryCache _cache;

    public GetTodosCountQueryHandlerTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        _cache = new TodosCountMemoryCache();
    }

    protected override IRequestHandler<GetTodosCountQuery, int> CommandHandler =>
        new GetTodosCountQueryHandler(_todosMok.Object, _cache, _currentServiceMok.Object);

    [Theory, FixtureInlineAutoData]
    public async Task Should_BeValid_When_CountTodos(GetTodosCountQuery command, Guid userId)
    {
        _currentServiceMok.SetupGet(p => p.CurrentUserId).Returns(userId);

        var todo = TestFixture.Build<Todo>().Create();
        todo.OwnerId = userId;
        _todosMok.Setup(
            p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
        ).ReturnsAsync(todo);

        await AssertNotThrow(command);
    }
}
