using AutoFixture;
using Core.Application.Abstractions.Persistence.Repository.Writing;
using Core.Auth.Application.Abstractions.Service;
using Core.Tests;
using Core.Tests.Attributes;
using Core.Tests.Fixtures;
using Core.Tests.Helpers;
using Core.Users.Domain.Enums;
using MediatR;
using Moq;
using System.Linq.Expressions;
using Todos.Applications.Caches;
using Todos.Applications.Handlers.Commands.DeleteTodo;
using Todos.Applications.Handlers.Commands.UpdateTodo;
using Todos.Applications.Handlers.Queries.GetTodos;
using Todos.Domain;
using Xunit.Abstractions;

namespace Todos.UnitTests.Tests.Commands.DeleteTodo;

public class DeleteTodoCommandHandlerTest : RequestHandlerTestBase<DeleteTodoCommand>
{
    private readonly Mock<IBaseWriteRepository<Todo>> _todosMok = new();

    private readonly Mock<ICurrentUserService> _currentServiceMok = new();

    private readonly ICleanTodosCacheService _cleanTodosCacheService;

    public DeleteTodoCommandHandlerTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        _cleanTodosCacheService = new CleanTodosCacheService(new Mock<TodoMemoryCache>().Object, new Mock<TodosListMemoryCache>().Object, new Mock<TodosCountMemoryCache>().Object);
    }

    protected override IRequestHandler<DeleteTodoCommand> CommandHandler =>
        new DeleteTodoCommandHandler(_todosMok.Object, _currentServiceMok.Object, _cleanTodosCacheService);

    [Theory, FixtureInlineAutoData]
    public async Task Should_BeValid_When_DeleteOtherOwnerTodoByAdmin(DeleteTodoCommand command, Guid userId)
    {
        _currentServiceMok.SetupGet(p => p.CurrentUserId).Returns(userId);
        _currentServiceMok.Setup(p => p.UserInRole(ApplicationUserRolesEnum.Admin)).Returns(true);

        var todo = TestFixture.Build<Todo>().Create();
        todo.OwnerId = GuidHelper.GetNotEqualGiud(userId);
        _todosMok.Setup(
            p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
        ).ReturnsAsync(todo);

        await AssertNotThrow(command);
    }

    [Theory, FixtureInlineAutoData]
    public async Task Should_BeValid_When_DeleteByClient(DeleteTodoCommand command, Guid userId)
    {
        _currentServiceMok.SetupGet(p => p.CurrentUserId).Returns(userId);
        _currentServiceMok.Setup(p => p.UserInRole(ApplicationUserRolesEnum.Client)).Returns(true);

        var todo = TestFixture.Build<Todo>().Create();
        todo.OwnerId = userId;
        _todosMok.Setup(
            p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
        ).ReturnsAsync(todo);

        await AssertNotThrow(command);
    }

    [Theory, FixtureInlineAutoData]
    public async Task Should_ThrowForbidden_When_DeleteByClientOtherOwnerTodo(DeleteTodoCommand command, Guid userId)
    {
        _currentServiceMok.SetupGet(p => p.CurrentUserId).Returns(userId);
        _currentServiceMok.Setup(p => p.UserInRole(ApplicationUserRolesEnum.Client)).Returns(true);

        var todo = TestFixture.Build<Todo>().Create();
        todo.OwnerId = GuidHelper.GetNotEqualGiud(userId);

        _todosMok.Setup(
            p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
        ).ReturnsAsync(todo);

        await AssertThrowForbiddenFound(command);
    }

    [Theory, FixtureInlineAutoData]
    public async Task Should_ThrowNotFound_When_TodoNotFound(DeleteTodoCommand command)
    {

        _todosMok.Setup(
            p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
        ).ReturnsAsync(null as Todo);

        await AssertThrowNotFound(command);
    }
}
