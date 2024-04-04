using Core.Tests.Attributes;
using Core.Tests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todos.Applications.Handlers.Commands.UpdateTodo;
using Xunit.Abstractions;
using AutoFixture;
using Todos.Applications.Handlers.Commands.DeleteTodo;

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
        [FixtureInlineAutoData(0)]
        [FixtureInlineAutoData(-1)]
        public void Should_NotBeValid_When_TodoIdIsNotPositive(int todoId)
        {
            // Arrange
            var command = new UpdateTodoCommand
            {
                TodoId = todoId,
                Name = "Valid Name"
            };

            // Act & Assert
            AssertNotValid(command);
        }

        [Theory]
        [FixtureInlineAutoData("")]
        [FixtureInlineAutoData(null)]
        public void Should_NotBeValid_When_NameIsInvalid(string name)
        {
            // Arrange
            var command = new UpdateTodoCommand
            {
                TodoId = 1,
                Name = name
            };

            // Act & Assert
            AssertNotValid(command);
        }

        [Fact]
        public void Should_BeValid_When_TodoIdAndNameAreValid()
        {
            // Arrange
            var command = new UpdateTodoCommand
            {
                TodoId = 1,
                Name = "Valid Name"
            };

            // Act & Assert
            AssertValid(command);
        }
    }
}
