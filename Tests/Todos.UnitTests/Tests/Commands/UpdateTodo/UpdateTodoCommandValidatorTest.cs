using AutoFixture;
using Core.Tests;
using Core.Tests.Attributes;
using FluentValidation;
using Todos.Applications.Handlers.Commands.UpdateTodo;
using Xunit.Abstractions;

namespace Todos.UnitTests.Tests.Commands.UpdateTodo
{
    public class UpdateTodoCommandValidatorTest : ValidatorTestBase<UpdateTodoCommand>
    {
        public UpdateTodoCommandValidatorTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override IValidator<UpdateTodoCommand> TestValidator => 
            TestFixture.Create<UpdateTodoCommandValidator>();

        [Theory]
        [FixtureInlineAutoData(1)]
        [FixtureInlineAutoData(10)]
        [FixtureInlineAutoData(20)]
        public void Should_Valid_When_TodoIdIsValid(int id)
        {
            // arrange
            var command = new UpdateTodoCommand
            {
                TodoId = id,
                Name = "Test"
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
            var command = new UpdateTodoCommand
            {
                TodoId = id
            };

            // act & assert
            AssertNotValid(command);
        }

        [Theory]
        [FixtureInlineAutoData("Name")]
        [FixtureInlineAutoData("NameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameName")]
        [FixtureInlineAutoData("20")]
        public void Should_BeValid_When_NameIsValid(string name)
        {
            // arrange
            var command = new UpdateTodoCommand
            {
                TodoId = 1,
                Name = name
            };

            // act & assert
            AssertValid(command);
        }

        [Theory]
        [FixtureInlineAutoData("NameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameName")]
        [FixtureInlineAutoData("")]
        public void Should_NotBeValid_When_IncorrectName(string name)
        {
            // arrange
            var command = new UpdateTodoCommand
            {
                Name = name
            };

            // act & assert
            AssertNotValid(command);
        }
    }
}
