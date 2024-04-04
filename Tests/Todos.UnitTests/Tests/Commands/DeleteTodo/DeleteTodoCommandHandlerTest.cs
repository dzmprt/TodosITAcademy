using AutoMapper;
using Core.Application.Abstractions.Persistence.Repository.Writing;
using Core.Auth.Application.Abstractions.Service;
using Core.Tests.Fixtures;
using Core.Tests;
using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todos.Applications.Caches;
using Todos.Applications.DTOs;
using Todos.Applications.Handlers.Commands.UpdateTodo;
using Todos.Applications.Handlers.Queries.GetTodos;
using Todos.Domain;
using Xunit.Abstractions;
using Todos.Applications.Handlers.Commands.DeleteTodo;
using Core.Auth.Api;
using Core.Tests.Attributes;
using Core.Users.Domain.Enums;
using System.Linq.Expressions;

namespace Todos.UnitTests.Tests.Commands.DeleteTodo
{
    public class DeleteTodoCommandHandlerTest : RequestHandlerTestBase<DeleteTodoCommand>
    {
        private readonly Mock<IBaseWriteRepository<Todo>> _todosMok = new();

        private readonly Mock<ICurrentUserService> _currentServiceMok = new();

        private readonly ICleanTodosCacheService _cleanTodosCacheService;

        private readonly IMapper _mapper;

        public DeleteTodoCommandHandlerTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _mapper = new AutoMapperFixture(typeof(GetTodosQuery).Assembly).Mapper;
            _cleanTodosCacheService = new CleanTodosCacheService(new Mock<TodoMemoryCache>().Object, new Mock<TodosListMemoryCache>().Object, new Mock<TodosCountMemoryCache>().Object);
        }


        protected override IRequestHandler<DeleteTodoCommand> CommandHandler =>
            new DeleteTodoCommandHandler(_todosMok.Object, _currentServiceMok.Object, _cleanTodosCacheService);

        [Theory, FixtureInlineAutoData]
        public async Task Should_DeleteTodo_When_TodoExistsAndCurrentUserIsOwner(DeleteTodoCommand command, Guid userId)
        {
            // Arrange
            _currentServiceMok.SetupGet(p => p.CurrentUserId).Returns(userId);
            var todo = new Todo { OwnerId = userId };
            _todosMok.Setup(p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default))
                .ReturnsAsync(todo);

            // Act & Assert
            await AssertNotThrow(command);
        }

        [Theory, FixtureInlineAutoData]
        public async Task Should_DeleteTodo_When_TodoExistsAndCurrentUserIsAdmin(DeleteTodoCommand command, Guid userId)
        {
            // Arrange
            _currentServiceMok.SetupGet(p => p.CurrentUserId).Returns(userId);
            _currentServiceMok.Setup(p => p.UserInRole(ApplicationUserRolesEnum.Admin)).Returns(true);
            var todo = new Todo { OwnerId = Guid.NewGuid() };
            _todosMok.Setup(p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default))
                .ReturnsAsync(todo);

            // Act & Assert
            await AssertNotThrow(command);
        }

        [Theory, FixtureInlineAutoData]
        public async Task Should_ThrowForbidden_When_TodoExistsAndCurrentUserIsNotOwnerNorAdmin(DeleteTodoCommand command, Guid userId)
        {
            // Arrange
            _currentServiceMok.SetupGet(p => p.CurrentUserId).Returns(userId);
            var todo = new Todo { OwnerId = Guid.NewGuid() };
            _todosMok.Setup(p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default))
                .ReturnsAsync(todo);

            // Act & Assert
            await AssertThrowForbiddenFound(command);
        }

        [Theory, FixtureInlineAutoData]
        public async Task Should_ThrowNotFound_When_TodoDoesNotExist(DeleteTodoCommand command)
        {
            // Arrange
            _todosMok.Setup(p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default))
                .ReturnsAsync(null as Todo);

            // Act & Assert
            await AssertThrowNotFound(command);
        }
    }
}
