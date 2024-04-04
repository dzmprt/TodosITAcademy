using AutoFixture;
using AutoMapper;
using Core.Application.Abstractions.Persistence.Repository.Read;
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
using Todos.Applications.DTOs;
using Todos.Applications.Handlers.Queries.GetTodo;
using Todos.Applications.Handlers.Queries.GetTodos;
using Todos.Domain;
using Xunit.Abstractions;

namespace Todos.UnitTests.Tests.Queries.GetTodo
{
    public class GetTodoQueryHandlerTest : RequestHandlerTestBase<GetTodoQuery, GetTodoDto>
    {
        private readonly Mock<IBaseReadRepository<Todo>> _todosMock = new();
        private readonly Mock<ICurrentUserService> _currentServiceMock = new();
        private readonly Mock<TodoMemoryCache> _mockTodoMemoryCache = new();
        private readonly IMapper _mapper;
        public GetTodoQueryHandlerTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _mapper = new AutoMapperFixture(typeof(GetTodosQuery).Assembly).Mapper;
        }

        protected override IRequestHandler<GetTodoQuery, GetTodoDto> CommandHandler =>
            new GetTodoQueryHandler(_todosMock.Object, _currentServiceMock.Object, _mapper, _mockTodoMemoryCache.Object);

        [Fact]
        public async Task Should_BeValid_When_GetTodoByAdmin()
        {
            // arrange
            var userId = Guid.NewGuid();
            _currentServiceMock.SetupGet(p => p.CurrentUserId).Returns(userId);

            var query = new GetTodoQuery();

            var todo = TestFixture.Build<Todo>().Create();

            _currentServiceMock.Setup(
                    p => p.UserInRole(ApplicationUserRolesEnum.Admin))
                .Returns(true);

            _todosMock.Setup(
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
            _currentServiceMock.SetupGet(p => p.CurrentUserId).Returns(userId);

            var query = new GetTodoQuery();

            var todo = TestFixture.Build<Todo>().Create();
            todo.OwnerId = userId;

            _currentServiceMock.Setup(
                    p => p.UserInRole(ApplicationUserRolesEnum.Admin))
                .Returns(false);

            _todosMock.Setup(
                p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
            ).ReturnsAsync(todo);

            // act and assert
            await AssertNotThrow(query);
        }

        [Theory, FixtureInlineAutoData]
        public async Task Should_ThrowForbidden_When_GetTodoWithOtherOwnerByClient(GetTodoQuery query, Guid userId)
        {
            // arrange
            _currentServiceMock.SetupGet(p => p.CurrentUserId).Returns(userId);
            _currentServiceMock.Setup(p => p.UserInRole(ApplicationUserRolesEnum.Client)).Returns(true);

            var todo = TestFixture.Build<Todo>().Create();
            todo.OwnerId = GuidHelper.GetNotEqualGiud(userId);

            _todosMock.Setup(
                p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
            ).ReturnsAsync(todo);

            // act and assert
            await AssertThrowForbiddenFound(query);
        }

        [Theory, FixtureInlineAutoData]
        public async Task Should_ThrowNotFound_When_TodoNotFound(GetTodoQuery query, Guid userId)
        {
            // arrange
            _currentServiceMock.SetupGet(p => p.CurrentUserId).Returns(userId);
            _todosMock.Setup(
                p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
            ).ReturnsAsync(null as Todo);

            // act and assert
            await AssertThrowNotFound(query);
        }
    }
}
