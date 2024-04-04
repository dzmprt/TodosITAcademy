using AutoMapper;
using Core.Application.Abstractions.Persistence.Repository.Writing;
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
using Todos.Applications.Handlers.Commands.UpdateTodo;
using Todos.Applications.Handlers.Queries.GetTodos;
using Todos.Domain;
using Xunit.Abstractions;
using Todos.Applications.Handlers.Commands.UpdateTodoIsDone;
using AutoFixture;
using Core.Tests.Attributes;
using Core.Tests.Helpers;
using Core.Users.Domain.Enums;
using System.Linq.Expressions;
using Todos.Applications.Handlers.Commands.DeleteTodo;

namespace Todos.UnitTests.Tests.Commands.UpdateTodoIsDone
{
    public class UpdateTodoIsDoneCommandHandlerTest : RequestHandlerTestBase<UpdateTodoIsDoneCommand, GetTodoDto>
    {
        private readonly Mock<IBaseWriteRepository<Todo>> _todosMok = new();

        private readonly Mock<ICurrentUserService> _currentServiceMok = new();

        private readonly ICleanTodosCacheService _cleanTodosCacheService;

        private readonly IMapper _mapper;

        public UpdateTodoIsDoneCommandHandlerTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _mapper = new AutoMapperFixture(typeof(GetTodosQuery).Assembly).Mapper;
            _cleanTodosCacheService = new CleanTodosCacheService(new Mock<TodoMemoryCache>().Object, new Mock<TodosListMemoryCache>().Object, new Mock<TodosCountMemoryCache>().Object);
        }

        protected override IRequestHandler<UpdateTodoIsDoneCommand, GetTodoDto> CommandHandler =>
            new UpdateTodoIsDoneCommandHandler(_todosMok.Object, _mapper, _currentServiceMok.Object, _cleanTodosCacheService);

        [Theory, FixtureInlineAutoData]
        public async Task Should_BeValid_When_UpdateIsDoneByAdmin(UpdateTodoIsDoneCommand command, Guid userId)
        {
            // arrange
            _currentServiceMok.SetupGet(p => p.CurrentUserId).Returns(userId);
            _currentServiceMok.Setup(p => p.UserInRole(ApplicationUserRolesEnum.Admin)).Returns(true);

            var todo = TestFixture.Build<Todo>().Create();
            todo.OwnerId = GuidHelper.GetNotEqualGiud(userId);
            todo.TodoId = command.TodoId;
            _todosMok.Setup(
                p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
            ).ReturnsAsync(todo);

            // act and assert
            await AssertNotThrow(command);
        }

        [Theory, FixtureInlineAutoData]
        public async Task Should_BeValid_When_UpdateIsDoneByOwner(UpdateTodoIsDoneCommand command, Guid userId)
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

        [Theory, FixtureInlineAutoData]
        public async Task Should_NotFound_When_UpdateIsDone_NotExistsTodo(UpdateTodoIsDoneCommand command)
        {
            // arrange
            Todo? todo = null;

            _todosMok.Setup(
                p => p.AsAsyncRead().SingleOrDefaultAsync(It.IsAny<Expression<Func<Todo, bool>>>(), default)
            ).ReturnsAsync(todo);

            // act and assert
            await AssertThrowNotFound(command);
        }

        [Theory, FixtureInlineAutoData]
        public async Task Should_Forbidden_When_UpdateIsDone_ByOtherUser(UpdateTodoIsDoneCommand command, Guid userId)
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
    }
}
