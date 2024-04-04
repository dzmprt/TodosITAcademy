using AutoFixture;
using Core.Application.Abstractions.Persistence.Repository.Read;
using Core.Auth.Application.Abstractions.Service;
using Core.Tests;
using Core.Users.Domain.Enums;
using MediatR;
using Moq;
using System.Linq.Expressions;
using Todos.Applications.Caches;
using Todos.Applications.Handlers.Queries.GetTodosCount;
using Todos.Domain;
using Xunit.Abstractions;

namespace Todos.UnitTests.Tests.Queries.GetTodosCount
{
    public class GetTodosCountQueryHandlerTest : RequestHandlerTestBase<GetTodosCountQuery, int>
    {
        private readonly Mock<IBaseReadRepository<Todo>> _todosMock = new();
        private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
        private readonly Mock<TodosCountMemoryCache> _mockTodosMemoryCache = new();
        public GetTodosCountQueryHandlerTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override IRequestHandler<GetTodosCountQuery, int> CommandHandler =>
            new GetTodosCountQueryHandler(_todosMock.Object, _mockTodosMemoryCache.Object, _currentUserServiceMock.Object);

        [Fact]
        public async Task Should_BeValid_When_GetTodosCountByAdmin()
        {
            // arrange
            var userId = Guid.NewGuid();
            _currentUserServiceMock.SetupGet(p => p.CurrentUserId).Returns(userId);

            var query = new GetTodosCountQuery();

            var todos = TestFixture.Build<Todo>().CreateMany(10).ToArray();
            var count = todos.Length;

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

            var todos = TestFixture.Build<Todo>().CreateMany(10).ToArray();
            var count = todos.Length;

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
