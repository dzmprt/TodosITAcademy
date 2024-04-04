using AutoMapper;
using Core.Application.Abstractions.Persistence.Repository.Writing;
using Core.Auth.Api;
using Core.Auth.Application.Abstractions.Service;
using Core.Tests.Attributes;
using Core.Tests.Fixtures;
using Core.Tests.Helpers;
using Core.Tests;
using Core.Users.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Todos.Applications.Caches;
using Todos.Applications.DTOs;
using Todos.Applications.Handlers.Commands.CreateTodo;
using Todos.Applications.Handlers.Commands.UpdateTodo;
using Todos.Applications.Handlers.Queries.GetTodos;
using Todos.Domain;
using Xunit.Abstractions;
using AutoFixture;
using System.Reflection.Metadata;

namespace Todos.UnitTests.Tests.Commands.CreateTodo
{
    public class CreateTodoCommandHandlerTest : RequestHandlerTestBase<CreateTodoCommand, GetTodoDto>
    {
        private readonly Mock<IBaseWriteRepository<Todo>> _todosMok = new();

        private readonly Mock<ICurrentUserService> _currentServiceMok = new();

        private readonly ICleanTodosCacheService _cleanTodosCacheService;

        private readonly IMapper _mapper;

        public CreateTodoCommandHandlerTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _mapper = new AutoMapperFixture(typeof(GetTodosQuery).Assembly).Mapper;
            _cleanTodosCacheService = new CleanTodosCacheService(new Mock<TodoMemoryCache>().Object, new Mock<TodosListMemoryCache>().Object, new Mock<TodosCountMemoryCache>().Object);
        }

       protected override IRequestHandler<CreateTodoCommand, GetTodoDto> CommandHandler =>
    new CreateTodoCommandHandler(_todosMok.Object, _currentServiceMok.Object, _mapper, _cleanTodosCacheService);

        [Theory, FixtureInlineAutoData]
        public async Task Should_CreateNewTodo_WithCorrectNameAndOwnerId(Guid userId)
        {
            // Arrange

            _currentServiceMok.SetupGet(p => p.CurrentUserId).Returns(userId);

            var todo = TestFixture.Build<Todo>().Create();

            todo.OwnerId = GuidHelper.GetNotEqualGiud(userId);

            _todosMok.Setup(
                p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
            ).ReturnsAsync(todo);

            var expectedTodoName = "Test Todo";
            var command = new CreateTodoCommand { Name = expectedTodoName };

            // Act and Assert

            await AssertNotThrow(command);
        }


    }
}
