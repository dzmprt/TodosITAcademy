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
using Todos.Domain;
using Xunit.Abstractions;

namespace Todos.UnitTests.Tests.Queries.GetTodo
{
    public class GetTodoQueryHandlerTest: RequestHandlerTestBase<GetTodoQuery, GetTodoDto>
    {
        private readonly Mock<IBaseReadRepository<Todo>> _todoMok = new();

        private readonly Mock<TodoMemoryCache> _todoMemoryCache = new();
        private readonly Mock<ICurrentUserService> _currentServiceMok = new();

        private readonly IMapper _mapper;
        public GetTodoQueryHandlerTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _mapper = new AutoMapperFixture(typeof(GetTodoQuery).Assembly).Mapper;
        }

        protected override IRequestHandler<GetTodoQuery, GetTodoDto> CommandHandler =>
            new GetTodoQueryHandler(_todoMok.Object,_currentServiceMok.Object,_mapper, _todoMemoryCache.Object);

        [Theory, FixtureInlineAutoData]
        public async Task Should_BeValid_When_TodoFounded(Guid userId)
        {
            // arrange
            _currentServiceMok.SetupGet(p => p.CurrentUserId).Returns(userId);
            Random rand = new();
            int todoId = rand.Next();
            var query = new GetTodoQuery()
            {
                TodoId = todoId,
                
            };
            var todo = TestFixture.Build<Todo>().Create();
            todo.TodoId=todoId;
            todo.OwnerId=userId;
            
            _todoMok.Setup(
                p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
            ).ReturnsAsync(todo);

            // act and assert
            await AssertNotThrow(query);
        }

        [Theory, FixtureInlineAutoData]
        public async Task Should_BeThrowNotFound_When_TodoNotFounded(Guid userId)
        {
            // arrange
            _currentServiceMok.SetupGet(p => p.CurrentUserId).Returns(userId);
            Random rand = new();
            int todoId = rand.Next();
            var query = new GetTodoQuery()
            {
                TodoId = todoId,

            };

            _todoMok.Setup(
                p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
            ).ReturnsAsync(null as Todo);

            // act and assert
            await AssertThrowNotFound(query);
        }
        [Theory, FixtureInlineAutoData]
        public async Task Should_BeValid_When_GetTodoByAdmin(GetTodoQuery query, Guid userId)
        {
            // arrange
            _currentServiceMok.SetupGet(p => p.CurrentUserId).Returns(userId);
            _currentServiceMok.Setup(p => p.UserInRole(ApplicationUserRolesEnum.Admin)).Returns(true);

            var todo = TestFixture.Build<Todo>().Create();
            todo.OwnerId = GuidHelper.GetNotEqualGiud(userId);
            _todoMok.Setup(
                p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
            ).ReturnsAsync(todo);

            // act and assert
            await AssertNotThrow(query);
        }

        [Theory, FixtureInlineAutoData]
        public async Task Should_BeValid_When_GetTodosByClient(GetTodoQuery query, Guid userId)
        {
            // arrange
            _currentServiceMok.SetupGet(p => p.CurrentUserId).Returns(userId);
            _currentServiceMok.Setup(p => p.UserInRole(ApplicationUserRolesEnum.Client)).Returns(true);

            var todo = TestFixture.Build<Todo>().Create();
            todo.OwnerId = userId;
            _todoMok.Setup(
                p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
            ).ReturnsAsync(todo);

            // act and assert
            await AssertNotThrow(query);
        }
       

        [Theory, FixtureInlineAutoData]
        public async Task Should_ThrowForbidden_When_GetTodosWithOtherOwnerByClient(GetTodoQuery query, Guid userId)
        {
            // arrange
            _currentServiceMok.SetupGet(p => p.CurrentUserId).Returns(userId);
            _currentServiceMok.Setup(p => p.UserInRole(ApplicationUserRolesEnum.Client)).Returns(true);

            var todo = TestFixture.Build<Todo>().Create();
            todo.OwnerId = GuidHelper.GetNotEqualGiud(userId);

            _todoMok.Setup(
                p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
            ).ReturnsAsync(todo);

            // act and assert
            await AssertThrowForbiddenFound(query);
        }
    }
}
