using AutoMapper;
using Core.Application.Abstractions.Persistence.Repository.Read;
using Core.Application.DTOs;
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
using Todos.Applications.Handlers.Queries.GetTodos;
using Todos.Domain;
using Xunit.Abstractions;
using Todos.Applications.Handlers.Queries.GetTodo;
using AutoFixture;
using Core.Tests.Attributes;
using Core.Tests.Helpers;
using Core.Users.Domain.Enums;
using System.Linq.Expressions;
using Todos.Applications.Handlers.Commands.DeleteTodo;

namespace Todos.UnitTests.Tests.Queries.GetTodo
{
    public class GetTodoQueryHandlerTest : RequestHandlerTestBase<GetTodoQuery, GetTodoDto>
    {
        private readonly Mock<IBaseReadRepository<Todo>> _todosMok = new();

        private readonly Mock<ICurrentUserService> _currentServiceMok = new();

        private readonly Mock<TodoMemoryCache> _mockTodosMemoryCache = new();

        private readonly IMapper _mapper;

        public GetTodoQueryHandlerTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _mapper = new AutoMapperFixture(typeof(GetTodosQuery).Assembly).Mapper;
        }

        protected override IRequestHandler<GetTodoQuery, GetTodoDto> CommandHandler =>
            new GetTodoQueryHandler(_todosMok.Object, _currentServiceMok.Object, _mapper, _mockTodosMemoryCache.Object);

        [Theory]
        [FixtureInlineAutoData]
        public async Task Should_BeValid_When_GetTodoById_ByAdmin(GetTodoQuery command, Guid userId)
        {
            // arrange
            _currentServiceMok.SetupGet(p => p.CurrentUserId).Returns(userId);
            _currentServiceMok.Setup(p => p.UserInRole(ApplicationUserRolesEnum.Admin)).Returns(true);
            var todo = TestFixture.Build<Todo>().Create();
            todo.OwnerId = userId;
            todo.TodoId = command.TodoId;
            _todosMok.Setup(
                p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
            ).ReturnsAsync(todo);

            // act and assert
            await AssertNotThrow(command);
        }
        [Theory]
        [FixtureInlineAutoData]
        public async Task Should_BeValid_When_GetTodoById_ByOwner(GetTodoQuery command, Guid userId)
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
        [Theory]
        [FixtureInlineAutoData]
        public async Task Should_BeNotValid_When_GetTodoById_ByOthweClient(GetTodoQuery command, Guid userId)
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

        [Theory]
        [FixtureInlineAutoData]
        public async Task Should_BeNotValid_When_TodoByIdNotFound(GetTodoQuery command, Guid userId)
        {
            // arrange
            _currentServiceMok.SetupGet(p => p.CurrentUserId).Returns(userId);
            _currentServiceMok.Setup(p => p.UserInRole(ApplicationUserRolesEnum.Client)).Returns(true);
            Todo? todo = null;
            _todosMok.Setup(
                p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
            ).ReturnsAsync(todo);

            // act and assert
            await AssertThrowNotFound(command);
        }
    }
}
