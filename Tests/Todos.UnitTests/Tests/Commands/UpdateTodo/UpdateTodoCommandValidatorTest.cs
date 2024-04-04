using Core.Tests.Attributes;
using Core.Tests;
using FluentValidation;
using Todos.Applications.Handlers.Commands.UpdateTodo;
using Xunit.Abstractions;
using AutoFixture;
using Todos.Applications.Handlers.Commands.CreateTodo;

namespace Todos.UnitTests.Tests.Commands.UpdateTodo
{
    public class UpdateTodoCommandValidatorTest : ValidatorTestBase<UpdateTodoCommand>
    {
        public UpdateTodoCommandValidatorTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override IValidator<UpdateTodoCommand> TestValidator
            => TestFixture.Create<UpdateTodoCommandValidator>();

        [Theory]
        [FixtureInlineAutoData(1)]
        [FixtureInlineAutoData(15)]
        [FixtureInlineAutoData(1555)]
        public void Should_BeValid_When_TodoIdIsValid(int todoId)
        {
            // arrange
            var command = new UpdateTodoCommand
            {
                TodoId = todoId,
                Name = "123"
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
            var command = new UpdateTodoCommand
            {
                TodoId = todoId,
                Name = "123"
            };

            // act & assert
            AssertNotValid(command);
        }

        [Theory]
        [FixtureInlineAutoData("1#*&^%$#@#$%±~`}{][\\|?/.,<>")]
        [FixtureInlineAutoData("12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890")]
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

        [Fact]
        public void Should_NotBeValid_When_NameIsGreatThen200()
        {
            // arrange
            var command = new UpdateTodoCommand
            {
                TodoId = 1,
                Name = "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890" +
                "12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901"
            };

            // act & assert
            AssertNotValid(command);
        }

        [Theory]
        [FixtureInlineAutoData(null)]
        [FixtureInlineAutoData("")]
        public void Should_NotBeValid_When_NameIsNullOrEmpty(string name)
        {
            // arrange
            var command = new UpdateTodoCommand
            {
                TodoId = 1,
                Name = name
            };

            // act & assert
            AssertNotValid(command);
        }
    }
}
