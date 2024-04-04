using AutoFixture;
using AutoMapper;
using Core.Application.Abstractions.Persistence.Repository.Writing;
using Core.Auth.Application.Abstractions.Service;
using Core.Tests;
using Core.Tests.Attributes;
using Core.Tests.Fixtures;
using Core.Tests.Helpers;
using MediatR;
using Moq;
using Todos.Applications.Caches;
using Todos.Applications.DTOs;
using Todos.Applications.Handlers.Commands.CreateTodo;
using Todos.Domain;
using Xunit.Abstractions;

namespace Todos.UnitTests.Tests.Commands.CreateTodo
{
    public class CreateTodoCommandHandlerTest : RequestHandlerTestBase<CreateTodoCommand, GetTodoDto>
    {
        private readonly Mock<IBaseWriteRepository<Todo>> _todosMock = new();

        private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();

        private readonly IMapper _mapper;

        private readonly ICleanTodosCacheService _cleanTodosCacheService;

        public CreateTodoCommandHandlerTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _mapper = new AutoMapperFixture(typeof(CreateTodoCommand).Assembly).Mapper;
            _cleanTodosCacheService = new CleanTodosCacheService(new Mock<TodoMemoryCache>().Object, new Mock<TodosListMemoryCache>().Object, new Mock<TodosCountMemoryCache>().Object);
        }

        protected override IRequestHandler<CreateTodoCommand, GetTodoDto> CommandHandler =>
            new CreateTodoCommandHandler(_todosMock.Object, _currentUserServiceMock.Object, _mapper, _cleanTodosCacheService);

        [Theory, FixtureInlineAutoData]
        public async Task Should_BeValid_When_CommandValid(CreateTodoCommand command, Guid userId)
        {
            // arrange
            _currentUserServiceMock.SetupGet(p => p.CurrentUserId).Returns(userId);

            var todo = TestFixture.Build<Todo>().Create();
            todo.OwnerId = GuidHelper.GetNotEqualGiud(userId);

            _todosMock.Setup(
                p => p.AddAsync(todo, default)
            ).ReturnsAsync(todo);

            // act and assert
            await AssertNotThrow(command);
        }
    }
}
