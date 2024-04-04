using AutoFixture;
using Core.Application.Abstractions.Persistence.Repository.Writing;
using Core.Auth.Application.Abstractions.Service;
using Core.Tests;
using Core.Tests.Attributes;
using Core.Tests.Helpers;
using Core.Users.Domain.Enums;
using MediatR;
using Moq;
using System.Linq.Expressions;
using Todos.Applications.Caches;
using Todos.Applications.Handlers.Commands.DeleteTodo;
using Todos.Domain;
using Xunit.Abstractions;

namespace Todos.UnitTests.Tests.Commands.DeleteTodo
{
    public class DeleteTodoCommandHandlerTest : RequestHandlerTestBase<DeleteTodoCommand>
    {
        private readonly Mock<IBaseWriteRepository<Todo>> _todosMock = new();

        private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();

        private readonly ICleanTodosCacheService _cleanTodosCacheService;

        public DeleteTodoCommandHandlerTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _cleanTodosCacheService = new CleanTodosCacheService(new Mock<TodoMemoryCache>().Object, new Mock<TodosListMemoryCache>().Object, new Mock<TodosCountMemoryCache>().Object);
        }

        protected override IRequestHandler<DeleteTodoCommand> CommandHandler => 
            new DeleteTodoCommandHandler(_todosMock.Object, _currentUserServiceMock.Object, _cleanTodosCacheService);

        [Theory, FixtureInlineAutoData]
        public async Task Should_BeValid_When_DeleteTodoByAdmin(DeleteTodoCommand command, Guid userId)
        {
            // arrange
            _currentUserServiceMock.SetupGet(p => p.CurrentUserId).Returns(userId);
            _currentUserServiceMock.Setup(p => p.UserInRole(ApplicationUserRolesEnum.Admin)).Returns(true);

            var todo = TestFixture.Build<Todo>().Create();
            todo.OwnerId = GuidHelper.GetNotEqualGiud(userId);

            _todosMock.Setup(
                p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
            ).ReturnsAsync(todo);

            // act and assert
            await AssertNotThrow(command);
        }

        [Theory, FixtureInlineAutoData]
        public async Task Should_BeValid_When_DeleteTodoByClient(DeleteTodoCommand command, Guid userId)
        {
            // arrange
            _currentUserServiceMock.SetupGet(p => p.CurrentUserId).Returns(userId);
            _currentUserServiceMock.Setup(p => p.UserInRole(ApplicationUserRolesEnum.Client)).Returns(true);

            var todo = TestFixture.Build<Todo>().Create();
            todo.OwnerId = userId;

            _todosMock.Setup(
                p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
            ).ReturnsAsync(todo);


            // act and assert
            await AssertNotThrow(command);
        }

        [Theory, FixtureInlineAutoData]
        public async Task Should_ThrowForbidden_When_DeleteTodoWithOtherOwnerByClient(DeleteTodoCommand command, Guid userId)
        {
            // arrange
            _currentUserServiceMock.SetupGet(p => p.CurrentUserId).Returns(userId);
            _currentUserServiceMock.Setup(p => p.UserInRole(ApplicationUserRolesEnum.Client)).Returns(true);

            var todo = TestFixture.Build<Todo>().Create();
            todo.OwnerId = GuidHelper.GetNotEqualGiud(userId);

            _todosMock.Setup(
                p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
            ).ReturnsAsync(todo);

            // act and assert
            await AssertThrowForbiddenFound(command);
        }

        [Theory, FixtureInlineAutoData]
        public async Task Should_ThrowNotFound_When_TodoNotFound(DeleteTodoCommand command)
        {
            // arrange

            _todosMock.Setup(
                p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
            ).ReturnsAsync(null as Todo);

            // act and assert
            await AssertThrowNotFound(command);
        }
    }
}
