using AutoFixture;
using Core.Application.Abstractions.Persistence.Repository.Read;
using Core.Auth.Application.Abstractions.Service;
using Core.Tests;
using Core.Tests.Attributes;
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
    public class GetTodosCountQueryHandlerTests:RequestHandlerTestBase<GetTodosCountQuery, int>
    {
        private readonly Mock<IBaseReadRepository<Todo>> _todosMok = new();
        private readonly Mock<ICurrentUserService> _currentServiceMok = new();
        private readonly Mock<TodosCountMemoryCache> _mockTodosCountMemoryCache = new();
        public GetTodosCountQueryHandlerTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override IRequestHandler<GetTodosCountQuery, int> CommandHandler =>
            new GetTodosCountQueryHandler(_todosMok.Object, _mockTodosCountMemoryCache.Object, _currentServiceMok.Object);

        [Theory]
        [FixtureInlineAutoData(10)]
        public async Task Should_BeValid_When_GetTodosCountByAdmin(int countTodos)
        {
            // arrange
            var userId = Guid.NewGuid();
            _currentServiceMok.SetupGet(p => p.CurrentUserId).Returns(userId);
            _currentServiceMok.Setup(p => p.UserInRole(ApplicationUserRolesEnum.Admin)).Returns(true);

            var query = new GetTodosCountQuery();

            var todos = TestFixture.Build<Todo>().CreateMany(countTodos).ToArray();
            
            var count = todos.Length;
           
            _todosMok.Setup(
                p => p.AsAsyncRead().CountAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
            ).ReturnsAsync(count);

            // act and assert
            await AssertNotThrow(query);
        }
        [Theory]
        [FixtureInlineAutoData(10)]
        public async Task Should_BeValid_When_GetTodosCountByClient(int countTodos)
        {
            // arrange
            var userId = Guid.NewGuid();
            _currentServiceMok.SetupGet(p => p.CurrentUserId).Returns(userId);

            var query = new GetTodosCountQuery();
            
            var todos = TestFixture.Build<Todo>().CreateMany(countTodos).ToArray();
            var count = todos.Length;
           
            _currentServiceMok.Setup(
                    p => p.UserInRole(ApplicationUserRolesEnum.Client))
                .Returns(true);
          
            _todosMok.Setup(
                p =>p.AsAsyncRead().CountAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
            ).ReturnsAsync(count);

            // act and assert
            await AssertNotThrow(query);
        }

    }
}
