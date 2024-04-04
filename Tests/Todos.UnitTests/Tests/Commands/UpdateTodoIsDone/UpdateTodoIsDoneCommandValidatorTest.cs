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

        protected override IValidator<UpdateTodoIsDoneCommand> TestValidator =>
            TestFixture.Create<UpdateTodoIsDoneCommandValidator>();

        [Theory]
        [FixtureInlineAutoData(1)]
        [FixtureInlineAutoData(10)]
        [FixtureInlineAutoData(20)]
        [FixtureInlineAutoData("123")]
        public void Should_Valid_When_TodoIdIsValid(int id)
        {
            // arrange
            var command = new UpdateTodoIsDoneCommand
            {
                TodoId = id
            };

            // act & assert
            AssertValid(command);
        }

        [Theory]
        [FixtureInlineAutoData(0)]
        [FixtureInlineAutoData(-1)]
        [FixtureInlineAutoData(-10)]
        public void Should_NotBeValid_When_TodoIdIncorrect(int id)
        {
            // arrange
            var command = new UpdateTodoIsDoneCommand
            {
                TodoId = id
            };

            // act & assert
            AssertNotValid(command);
        }
    }
}
