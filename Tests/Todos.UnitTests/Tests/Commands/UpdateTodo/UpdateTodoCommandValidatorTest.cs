using Core.Tests;
using AutoFixture;
using FluentValidation;
using Xunit.Abstractions;
using Todos.Applications.Handlers.Commands.UpdateTodo;

namespace Todos.UnitTests.Tests.Commands.UpdateTodo
{
    public class UpdateTodoCommandValidatorTest : ValidatorTestBase<UpdateTodoCommand>
    {
        public UpdateTodoCommandValidatorTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override IValidator<UpdateTodoCommand> TestValidator => TestFixture.Create<UpdateTodoCommandValidator>();

        [Theory]
        [InlineData("1", 1)]
        [InlineData("Name", 123)]
        [InlineData("12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890", 1)]
        public void Should_BeValid_When_RequestIsValid(string name, int id)
        {
            // arrange
            var update = new UpdateTodoCommand
            {
                IsDone = true,
                Name = name,
                TodoId = id,
            };

            // act & assert
            AssertValid(update);
        }
        
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(int.MinValue)]
        
        
        public void Should_NotBeValid_When_RequestIsNotValid(int todoId)
        {
            // arrange
            var update = new UpdateTodoCommand
            {
                IsDone = true,
                Name = "111",
                TodoId = todoId
            };

            // act & assert
            AssertNotValid(update);
        }
        
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901")]
        public void Should_NotBeValid_When_NameIsNotValid(string? name)
        {
            // arrange
            var update = new UpdateTodoCommand
            {
                IsDone = true,
                Name = name,
                TodoId = 1
            };

            // act & assert
            AssertNotValid(update);
        }
    }
}
