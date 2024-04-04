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
using Todos.Applications.Handlers.Queries.GetTodos;
using Todos.Applications.Handlers.Queries.GetTodosCount;
using Todos.Domain;
using Xunit.Abstractions;
using Core.Users.Domain.Enums;
using System.Linq.Expressions;

namespace Todos.UnitTests.Tests.Queries.GetTodosCount
{
    public class GetTodosCountQueryHandlerTest : RequestHandlerTestBase<GetTodosCountQuery, int>
    {
        private readonly Mock<IBaseReadRepository<Todo>> _todosMok = new();
        private readonly Mock<ICurrentUserService> _currentServiceMok = new();
        private readonly Mock<TodosCountMemoryCache> _mockTodosCountMemoryCache = new();
        private readonly IMapper _mapper;

        public GetTodosCountQueryHandlerTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _mapper = new AutoMapperFixture(typeof(GetTodosQuery).Assembly).Mapper;
        }

        protected override IRequestHandler<GetTodosCountQuery, int> CommandHandler =>
            new GetTodosCountQueryHandler(_todosMok.Object, _mockTodosCountMemoryCache.Object, _currentServiceMok.Object);

        [Fact]
        public async Task Should_ReturnCountForAdmin()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _currentServiceMok.SetupGet(p => p.CurrentUserId).Returns(userId);

            var query = new GetTodosCountQuery();

            var count = 10; 

            _currentServiceMok.Setup(p => p.UserInRole(ApplicationUserRolesEnum.Admin)).Returns(true);

            _todosMok.Setup(p => p.AsAsyncRead().CountAsync(
                It.IsAny<Expression<Func<Todo, bool>>>(), default))
                .ReturnsAsync(count);

            // Act and Assert
            await AssertNotThrow(query);
        }

        [Fact]
        public async Task Should_ReturnCountForClient()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _currentServiceMok.SetupGet(p => p.CurrentUserId).Returns(userId);

            var query = new GetTodosCountQuery();

            var count = 10; 

            _currentServiceMok.Setup(p => p.UserInRole(ApplicationUserRolesEnum.Admin)).Returns(false);

            _todosMok.Setup(p => p.AsAsyncRead().CountAsync(
                It.IsAny<Expression<Func<Todo, bool>>>(), default))
                .ReturnsAsync(count);

            // Act and Assert
            await AssertNotThrow(query);
        }
    }

}
