using AutoFixture;
using Core.Tests;
using Core.Tests.Attributes;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todos.Applications.Handlers.Commands.CreateTodo;
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
        [FixtureInlineAutoData(0)]
        [FixtureInlineAutoData(-1)]
        public void Should_NotBeValid_When_TodoIdIsNotPositiveOrZero(int todoId)
        {
            // Arrange
            var command = new DeleteTodoCommand
            {
                TodoId = todoId
            };

            // Act & Assert
            AssertNotValid(command);
        }

        [Theory]
        [FixtureInlineAutoData(1)]
        [FixtureInlineAutoData(10)]
        public void Should_BeValid_When_TodoIdIsValid(int todoId)
        {
            // Arrange
            var command = new DeleteTodoCommand
            {
                TodoId = todoId
            };

            // Act & Assert
            AssertValid(command);
        }
    }
}
