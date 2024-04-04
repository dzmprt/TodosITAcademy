using AutoFixture;
using Core.Tests;
using Core.Tests.Attributes;
using FluentValidation;
using Todos.Applications.Handlers.Commands.CreateTodo;
using Xunit.Abstractions;

namespace Todos.UnitTests.Tests.Commands.CreateTodo
{
    public class CreateTodoCommandValidatorTest : ValidatorTestBase<CreateTodoCommand>
    {
        public CreateTodoCommandValidatorTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override IValidator<CreateTodoCommand> TestValidator =>
            TestFixture.Create<CreateTodoCommandValidator>();

        [Theory]
        [FixtureInlineAutoData("Name")]
        [FixtureInlineAutoData("NameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameName")]
        [FixtureInlineAutoData("20")]
        public void Should_BeValid_When_NameIsValid(string name)
        {
            // arrange
            var command = new CreateTodoCommand
            {
                Name = name
            };

            // act & assert
            AssertValid(command);
        }

        [Theory]
        [FixtureInlineAutoData("NameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameNameName")]
        [FixtureInlineAutoData(null)]
        [FixtureInlineAutoData("")]
        public void Should_NotBeValid_When_IncorrectName(string name)
        {
            // arrange
            var command = new CreateTodoCommand
            {
                Name = name
            };

            // act & assert
            AssertNotValid(command);
        }
    }
}
