using AutoFixture;
using AutoMapper;
using Core.Application.Abstractions.Persistence.Repository.Read;
using Core.Application.DTOs;
using Core.Auth.Application.Abstractions.Service;
using Core.Tests;
using Core.Tests.Attributes;
using Core.Tests.Fixtures;
using Core.Users.Domain.Enums;
using FluentValidation;
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
using Todos.Applications.Handlers.Queries.GetTodos;
using Todos.Applications.Handlers.Queries.GetTodosCount;
using Todos.Domain;
using Xunit.Abstractions;

namespace Todos.UnitTests.Tests.Queries.GetTodoCount
{
    public class GetTodoCountQueryHandlerTest : RequestHandlerTestBase<GetTodosQuery, BaseListDto<GetTodoDto>>
    {
        private readonly Mock<IBaseReadRepository<Todo>> _todosMok = new();

        private readonly Mock<ICurrentUserService> _currentServiceMok = new();

        private readonly Mock<TodosListMemoryCache> _mockTodosMemoryCache = new();

        private readonly IMapper _mapper;

        public GetTodoCountQueryHandlerTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _mapper = new AutoMapperFixture(typeof(GetTodosQuery).Assembly).Mapper;
        }

        protected override IRequestHandler<GetTodosQuery, BaseListDto<GetTodoDto>> CommandHandler =>
            new GetTodosQueryHandler(_todosMok.Object, _currentServiceMok.Object, _mapper, _mockTodosMemoryCache.Object);

        [Fact]
        public async Task Should_BeValid_When_GetTodosByAdmin()
        {
            // arrange
            var userId = Guid.NewGuid();
            _currentServiceMok.SetupGet(p => p.CurrentUserId).Returns(userId);

            var query = new GetTodosQuery();

            var todos = TestFixture.Build<Todo>().CreateMany(10).ToArray();
            var count = todos.Length;

            _currentServiceMok.Setup(
                    p => p.UserInRole(ApplicationUserRolesEnum.Admin))
                .Returns(true);

            _todosMok.Setup(
                p => p.AsAsyncRead().CountAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
            ).ReturnsAsync(count);

            // act and assert
            await AssertNotThrow(query);
        }

        [Fact]
        public async Task Should_BeValid_When_GetTodosByClient()
        {
            // arrange
            var userId = Guid.NewGuid();
            _currentServiceMok.SetupGet(p => p.CurrentUserId).Returns(userId);

            var query = new GetTodosQuery();

            var todos = TestFixture.Build<Todo>().CreateMany(10).ToArray();
            var count = todos.Length;


            _currentServiceMok.Setup(
                    p => p.UserInRole(ApplicationUserRolesEnum.Client))
                .Returns(false);

            _todosMok.Setup(
                p => p.AsAsyncRead().CountAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
            ).ReturnsAsync(count);

            // act and assert
            await AssertNotThrow(query);
        }
    }
}
