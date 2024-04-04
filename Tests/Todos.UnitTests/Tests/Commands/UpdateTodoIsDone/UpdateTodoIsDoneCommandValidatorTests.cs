using Core.Tests;
using Core.Tests.Attributes;
using FluentValidation;
using Todos.Applications.Handlers.Commands.UpdateTodoIsDone;
using Xunit.Abstractions;

namespace Todos.UnitTests.Tests.Commands.UpdateTodoIsDone
{
    public class UpdateTodoIsDoneCommandValidatorTests : ValidatorTestBase<UpdateTodoIsDoneCommand>
    {
        public UpdateTodoIsDoneCommandValidatorTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override IValidator<UpdateTodoIsDoneCommand> TestValidator =>
        new UpdateTodoIsDoneCommandValidator();

        [Theory]
        [FixtureInlineAutoData(1)]
        [FixtureInlineAutoData(int.MaxValue)]
        public void Should_BeValid_When_RequestIsValid(int todoId)
        {
            // arrange
            var query = new UpdateTodoIsDoneCommand()
            {
                TodoId = todoId,

            };

            // act & assert
            AssertValid(query);
        }

        [Theory]
        [FixtureInlineAutoData(0)]
        [FixtureInlineAutoData(-1)]
        public void Should_NotValid_When_RequesIsNotValid(int todoId)
        {
            // arrange
            var query = new UpdateTodoIsDoneCommand()
            {
                TodoId = todoId,

            };

            // act & assert
            AssertNotValid(query);
        }
    }
}
