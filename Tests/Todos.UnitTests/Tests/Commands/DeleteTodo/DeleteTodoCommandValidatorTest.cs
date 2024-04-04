using AutoFixture;
using Core.Tests;
using Core.Tests.Attributes;
using FluentValidation;
using Todos.Applications.Handlers.Commands.DeleteTodo;
using Xunit.Abstractions;

namespace Todos.UnitTests.Tests.Commands.DeleteTodo
{
    public class DeleteTodoCommandValidatorTest : ValidatorTestBase<DeleteTodoCommand>
    {
        public DeleteTodoCommandValidatorTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override IValidator<DeleteTodoCommand> TestValidator =>
            TestFixture.Create<DeleteTodoCommandValidator>();

        [Theory]
        [FixtureInlineAutoData(1)]
        [FixtureInlineAutoData(10)]
        [FixtureInlineAutoData(20)]
        [FixtureInlineAutoData("123")]
        public void Should_Valid_When_TodoIdIsValid(int id)
        {
            // arrange
            var command = new DeleteTodoCommand
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
            var command = new DeleteTodoCommand
            {
                TodoId = id
            };

            // act & assert
            AssertNotValid(command);
        }
    }
}
