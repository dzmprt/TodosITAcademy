using AutoMapper;
using Core.Application.Abstractions.Persistence.Repository.Writing;
using Core.Auth.Application.Abstractions.Service;
using Core.Tests;
using MediatR;
using Todos.Applications.Caches;
using Todos.Applications.DTOs;
using Todos.Applications.Handlers.Commands.UpdateTodoIsDone;
using Todos.Domain;
using Xunit.Abstractions;
using Moq;
using Core.Tests.Fixtures;
using Todos.Applications.Handlers.Queries.GetTodos;
using AutoFixture;
using Core.Tests.Attributes;
using Core.Tests.Helpers;
using Todos.Applications.Handlers.Commands.UpdateTodo;
using Core.Users.Domain.Enums;
using System.Linq.Expressions;

namespace Todos.UnitTests.Tests.Commands.UpdateTodoIsDone
{
    public class UpdateTodoIsDoneCommandHandlerTest : RequestHandlerTestBase<UpdateTodoIsDoneCommand, GetTodoDto>
    {
        private readonly Mock<IBaseWriteRepository<Todo>> _todosMock = new();

        private readonly IMapper _mapper;

        private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();

        private readonly ICleanTodosCacheService _cleanTodosCacheService;
        public UpdateTodoIsDoneCommandHandlerTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _mapper = new AutoMapperFixture(typeof(GetTodosQuery).Assembly).Mapper;
            _cleanTodosCacheService = new CleanTodosCacheService(new Mock<TodoMemoryCache>().Object, new Mock<TodosListMemoryCache>().Object, new Mock<TodosCountMemoryCache>().Object);
        }

        protected override IRequestHandler<UpdateTodoIsDoneCommand, GetTodoDto> CommandHandler =>
            new UpdateTodoIsDoneCommandHandler(_todosMock.Object, _mapper, _currentUserServiceMock.Object, _cleanTodosCacheService);

        [Theory, FixtureInlineAutoData]
        public async Task Should_BeValid_When_GetTodoByAdmin(UpdateTodoIsDoneCommand command, Guid userId)
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
        public async Task Should_BeValid_When_GetTodoByClient(UpdateTodoIsDoneCommand command, Guid userId)
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
        public async Task Should_ThrowForbidden_When_GetTodoWithOtherOwnerByClient(UpdateTodoIsDoneCommand command, Guid userId)
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
        public async Task Should_ThrowNotFound_When_TodoNotFound(UpdateTodoIsDoneCommand command)
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
