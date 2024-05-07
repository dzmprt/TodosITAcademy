using AutoFixture;
using Core.Tests;
using Core.Tests.Attributes;
using FluentValidation;
using Todos.Applications.Handlers.Commands.CreateTodo;
using Xunit.Abstractions;

namespace Todos.UnitTests.Tests.Commands.CreateTodo
{
    public class CreateTodoCommandValidatorTests : ValidatorTestBase<CreateTodoCommand>
    {
        public CreateTodoCommandValidatorTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override IValidator<CreateTodoCommand> TestValidator =>
            TestFixture.Create<CreateTodoCommandValidator>();

        [Theory]
        [FixtureInlineAutoData(1)]
        [FixtureInlineAutoData(200)]
        public void Should_BeValid_When_NameIsValid(int countLetters)
        {
            // arrange
            var query = new CreateTodoCommand()
            {
                Name = new string('t', countLetters),
               
            };

            // act & assert
            AssertValid(query);
        }

        [Theory]
        [FixtureInlineAutoData(0)]
        [FixtureInlineAutoData(201)]
        public void Should_NotValid_When_NameIsNotValid(int countLetters)
        {
            // arrange
            var query = new CreateTodoCommand()
            {
                Name = countLetters == 0 ? string.Empty : new string('t', countLetters),
               
            };

            // act & assert
            AssertNotValid(query);
        }
    }
}
