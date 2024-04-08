using AutoFixture;
using Core.Application.Abstractions.Persistence.Repository.Writing;
using Core.Auth.Application.Abstractions.Service;
using Core.Tests;
using Core.Tests.Attributes;
using Core.Tests.Helpers;
using Core.Users.Domain.Enums;
using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Todos.Applications.Caches;
using Todos.Applications.DTOs;
using Todos.Applications.Handlers.Commands.DeleteTodo;
using Todos.Applications.Handlers.Commands.UpdateTodo;
using Todos.Domain;
using Xunit.Abstractions;

namespace Todos.UnitTests.Tests.Commands.DeleteTodo
{
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
        public async Task Should_BeValid_When_DeleteTodoByAdmin(DeleteTodoCommand command, Guid userId)
        {

            // arrange
            _currentServiceMok.SetupGet(p => p.CurrentUserId).Returns(userId);
            _currentServiceMok.Setup(p => p.UserInRole(ApplicationUserRolesEnum.Admin)).Returns(true);
            var todo = TestFixture.Build<Todo>().Create();
            todo.OwnerId = GuidHelper.GetNotEqualGiud(userId);
            todo.TodoId = command.TodoId;
            _todosMok.Setup(
                p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
            ).ReturnsAsync(todo);

            // act and assert
            await AssertNotThrow(command);
        }

        [Theory, FixtureInlineAutoData]
        public async Task Should_BeValid_When_DeleteTodoByOwner(DeleteTodoCommand command, Guid userId)
        {
            // arrange
            _currentServiceMok.SetupGet(p => p.CurrentUserId).Returns(userId);
            _currentServiceMok.Setup(p => p.UserInRole(ApplicationUserRolesEnum.Client)).Returns(true);
            var todo = TestFixture.Build<Todo>().Create();
            todo.OwnerId = userId;
            todo.TodoId = command.TodoId;
            _todosMok.Setup(
                p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
            ).ReturnsAsync(todo);

            // act and assert
            await AssertNotThrow(command);
        }

        [Theory, FixtureInlineAutoData]
        public async Task Should_Throw_NotFound_When_DeletingTodo_NotExists(DeleteTodoCommand command)
        {
            // arrange
            Todo? todo = null;
            _todosMok.Setup(
                p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
            ).ReturnsAsync(todo);

            // act and assert
            await AssertThrowNotFound(command);
        }

        [Theory, FixtureInlineAutoData]
        public async Task Should_Throw_Forbidden_When_DeleteTodoByOtherUser(DeleteTodoCommand command, Guid userId)
        {
            // arrange
            _currentServiceMok.SetupGet(p => p.CurrentUserId).Returns(userId);
            _currentServiceMok.Setup(p => p.UserInRole(ApplicationUserRolesEnum.Client)).Returns(true);
            var todo = TestFixture.Build<Todo>().Create();
            todo.OwnerId = GuidHelper.GetNotEqualGiud(userId);
            todo.TodoId = command.TodoId;
            _todosMok.Setup(
                p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
            ).ReturnsAsync(todo);

            // act and assert
            await AssertThrowForbidden(command);
        }
    }
}
