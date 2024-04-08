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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Todos.Applications.Caches;
using Todos.Applications.DTOs;
using Todos.Applications.Handlers.Commands.UpdateTodo;
using Todos.Applications.Handlers.Queries.GetTodo;
using Todos.Applications.Handlers.Queries.GetTodos;
using Todos.Domain;
using Xunit.Abstractions;

namespace Todos.UnitTests.Tests.Queries.GetTodo
{
    public class GetTodoQueryHandlerTest : RequestHandlerTestBase<GetTodoQuery, GetTodoDto>
    {
        private readonly Mock<IBaseReadRepository<Todo>> _todoMock = new();
        private readonly Mock<ICurrentUserService> _currentServiceMok = new();
        private readonly Mock<TodoMemoryCache> _todoMemoryCacheMock = new();
        private readonly IMapper _mapper;
        public GetTodoQueryHandlerTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _mapper = new AutoMapperFixture(typeof(GetTodoQuery).Assembly).Mapper;
        }

        protected override IRequestHandler<GetTodoQuery, GetTodoDto> CommandHandler
            => new GetTodoQueryHandler(_todoMock.Object, _currentServiceMok.Object, _mapper, _todoMemoryCacheMock.Object );

        [Theory, FixtureInlineAutoData]
        public async Task Shoud_BeValid_When_GetByClient(GetTodoQuery query, Guid userId)
        {
            _currentServiceMok.SetupGet(p => p.CurrentUserId).Returns(userId);
            _currentServiceMok.Setup(p => p.UserInRole(ApplicationUserRolesEnum.Client)).Returns(true);

            var todo = TestFixture.Build<Todo>().Create();
            todo.OwnerId = userId;

            _todoMock.Setup(
                p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
                ).ReturnsAsync(todo);

            await AssertNotThrow(query);
        }


        [Theory, FixtureInlineAutoData]
        public async Task Shoud_BeValid_When_GetByAdmin(GetTodoQuery query, Guid userId)
        {
            _currentServiceMok.SetupGet(p => p.CurrentUserId).Returns(userId);
            _currentServiceMok.Setup(p => p.UserInRole(ApplicationUserRolesEnum.Admin)).Returns(true);

            var todo = TestFixture.Build<Todo>().Create();

            _todoMock.Setup(
                p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
                ).ReturnsAsync(todo);

            await AssertNotThrow(query);
        }


        [Theory, FixtureInlineAutoData]
        public async Task Shoud_BeForbiden_When_GetByOtherClient(GetTodoQuery query, Guid userId)
        {
            _currentServiceMok.SetupGet(p => p.CurrentUserId).Returns(userId);
            _currentServiceMok.Setup(p => p.UserInRole(ApplicationUserRolesEnum.Client)).Returns(true);

            var todo = TestFixture.Build<Todo>().Create();

            _todoMock.Setup(
                p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
                ).ReturnsAsync(todo);

            await AssertThrowForbiddenFound(query);
        }

        [Theory, FixtureInlineAutoData]
        public async Task Shoud_BeNotValid_When_NotFound(GetTodoQuery query, Guid userId)
        {
            _currentServiceMok.SetupGet(p=>p.CurrentUserId).Returns(userId);

            _todoMock.Setup(
                p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
                ).ReturnsAsync(null as Todo);

            await AssertThrowNotFound(query);
        }
    }
}
