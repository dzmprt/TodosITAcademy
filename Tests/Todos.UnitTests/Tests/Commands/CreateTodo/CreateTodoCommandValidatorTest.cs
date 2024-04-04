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
        [FixtureInlineAutoData("1#*&^%$#@#$%±~`}{][\\|?/.,<>")]
        [FixtureInlineAutoData("12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890")]
        public void Should_BeValid_When_NameIsValid(string name)
        {
            // arrange
            var query = new CreateTodoCommand
            {
                Name = name
            };

            // act & assert
            AssertValid(query);
        }

        [Fact]
        public void Should_NotBeValid_When_NameIsGreatThen200()
        {
            // arrange
            var query = new CreateTodoCommand
            { 
                Name = "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890" +
                "12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901"
            };

            // act & assert
            AssertNotValid(query);
        }

        [Theory]
        [FixtureInlineAutoData(null)]
        [FixtureInlineAutoData("")]
        public void Should_NotBeValid_When_NameIsNullOrEmpty(string name)
        {
            // arrange
            var query = new CreateTodoCommand 
            { 
                Name = name 
            };

            // act & assert
            AssertNotValid(query);
        }

    }
}
