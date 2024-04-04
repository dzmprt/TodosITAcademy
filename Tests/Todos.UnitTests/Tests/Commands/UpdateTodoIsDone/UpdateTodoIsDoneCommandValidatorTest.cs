using AutoFixture;
using Core.Tests;
using Core.Tests.Attributes;
using FluentValidation;
using Todos.Applications.Handlers.Commands.UpdateTodoIsDone;
using Xunit.Abstractions;

namespace Todos.UnitTests.Tests.Commands.UpdateTodoIsDone
{
    public class UpdateTodoIsDoneCommandValidatorTest : ValidatorTestBase<UpdateTodoIsDoneCommand>
    {
        public UpdateTodoIsDoneCommandValidatorTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override IValidator<UpdateTodoIsDoneCommand> TestValidator
            => TestFixture.Create<UpdateTodoIsDoneCommandValidator>();

        [Theory]
        [FixtureInlineAutoData(1)]
        [FixtureInlineAutoData(15)]
        [FixtureInlineAutoData(1555)]
        public void Should_BeValid_When_TodoIdIsValid(int todoId)
        {
            // arrange
            var command = new UpdateTodoIsDoneCommand
            {
                TodoId = todoId,
            };

            // act & assert
            AssertValid(command);
        }

        [Theory]
        [FixtureInlineAutoData(0)]
        [FixtureInlineAutoData(-1)]
        public void Should_NotBeValid_When_IncorrectTodoId(int todoId)
        {
            // arrange
            var command = new UpdateTodoIsDoneCommand
            {
                TodoId = todoId,
            };

            // act & assert
            AssertNotValid(command);
        }
    }
}
