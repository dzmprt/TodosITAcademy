using AutoMapper;
using Core.Application.Abstractions.Persistence.Repository.Read;
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
using Todos.Applications.Handlers.Queries.GetTodo;
using Todos.Domain;
using Xunit.Abstractions;
using System.Linq.Expressions;
using Core.Application.DTOs;
using Todos.Applications.Handlers.Queries.GetTodos;
using Core.Application.Exceptions;
using Core.Auth.Application.Exceptions;
using AutoFixture;
using Core.Users.Domain.Enums;

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
            _mapper = new AutoMapperFixture(typeof(GetTodoQuery).Assembly).Mapper;
        }

        protected override IRequestHandler<GetTodoQuery, GetTodoDto> CommandHandler =>
            new GetTodoQueryHandler(_todosMok.Object, _currentServiceMok.Object, _mapper, _mockTodosMemoryCache.Object);
      
        [Fact]
        public async Task Should_BeValid_When_GetTodoByAdmin()
        {
            // arrange
            var userId = Guid.NewGuid();
            _currentServiceMok.SetupGet(p => p.CurrentUserId).Returns(userId);

            var query = new GetTodoQuery();

            var todo = TestFixture.Build<Todo>().Create();
            _currentServiceMok.Setup(p => p.UserInRole(ApplicationUserRolesEnum.Admin)).Returns(true);
            _todosMok.Setup(p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default))
                .ReturnsAsync(todo);

            // act and assert
            await AssertNotThrow(query);
        }

        [Fact]
        public async Task Should_ThrowForbiddenException_When_GetTodoByClient()
        {
            // arrange
            var userId = Guid.NewGuid();
            _currentServiceMok.SetupGet(p => p.CurrentUserId).Returns(userId);

            var query = new GetTodoQuery();

            var todo = TestFixture.Build<Todo>().Create();

            _currentServiceMok.Setup(
                    p => p.UserInRole(ApplicationUserRolesEnum.Admin))
                .Returns(false);

            _todosMok.Setup(
                p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default))
                .ReturnsAsync(todo);

            // act and assert
            await AssertThrowForbiddenFound(query);
        }

    }
}
