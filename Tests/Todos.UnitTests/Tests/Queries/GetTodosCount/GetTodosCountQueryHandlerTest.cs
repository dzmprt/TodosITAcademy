using AutoFixture;
using Core.Application.Abstractions.Persistence.Repository.Read;
using Core.Auth.Application.Abstractions.Service;
using Core.Tests;
using Core.Tests.Fixtures;
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
using Todos.Applications.Handlers.Queries.GetTodos;
using Todos.Applications.Handlers.Queries.GetTodosCount;
using Todos.Domain;
using Xunit.Abstractions;

namespace Todos.UnitTests.Tests.Queries.GetTodosCount
{
    public class GetTodosCountQueryHandlerTest : RequestHandlerTestBase<GetTodosCountQuery, int>
    {
        private readonly Mock<IBaseReadRepository<Todo>> _todosMock = new();

        private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();

        private readonly Mock<TodosCountMemoryCache> _todosCountMemoryCacheMock = new();

        public GetTodosCountQueryHandlerTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override IRequestHandler<GetTodosCountQuery, int> CommandHandler =>
            new GetTodosCountQueryHandler(_todosMock.Object, _todosCountMemoryCacheMock.Object, _currentUserServiceMock.Object);

        [Fact]
        public async Task Should_BeValid_When_GetTodosCountByAdmin()
        {
            // arrange
            var userId = Guid.NewGuid();
            _currentUserServiceMock.SetupGet(p => p.CurrentUserId).Returns(userId);

            var query = new GetTodosCountQuery();

            var count = 10;

            _currentUserServiceMock.Setup(
                    p => p.UserInRole(ApplicationUserRolesEnum.Admin))
                .Returns(true);

            _todosMock.Setup(
                p => p.AsAsyncRead().CountAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
            ).ReturnsAsync(count);

            // act and assert
            await AssertNotThrow(query);
        }

        [Fact]
        public async Task Should_BeValid_When_GetTodosCountByClient()
        {
            // arrange
            var userId = Guid.NewGuid();
            _currentUserServiceMock.SetupGet(p => p.CurrentUserId).Returns(userId);

            var query = new GetTodosCountQuery();

            var count = 10;

            _currentUserServiceMock.Setup(
                    p => p.UserInRole(ApplicationUserRolesEnum.Admin))
                .Returns(false);

            _todosMock.Setup(
                p => p.AsAsyncRead().CountAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
            ).ReturnsAsync(count);

            // act and assert
            await AssertNotThrow(query);
        }
    }
}
