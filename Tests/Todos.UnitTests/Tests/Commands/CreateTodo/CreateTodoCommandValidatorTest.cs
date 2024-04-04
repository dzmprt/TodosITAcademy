using Core.Tests.Attributes;
using Core.Tests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todos.Applications.Handlers.Queries.GetTodos;
using Xunit.Abstractions;
using Todos.Applications.Handlers.Commands.CreateTodo;
using AutoFixture;

namespace Todos.UnitTests.Tests.Commands.CreateTodo
{
    public class CreateTodoCommandValidatorTest : ValidatorTestBase<CreateTodoCommand>
    {
        public CreateTodoCommandValidatorTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override IValidator<CreateTodoCommand> TestValidator =>
            TestFixture.Create<CreateTodoCommandValidator>();


        [Fact]
        public void Should_BeValid_When_NameIsValid()
        {
            // Arrange
            var command = new CreateTodoCommand
            {
                Name = "Valid Todo Name"
            };

            // Act & Assert
            AssertValid(command);
        }

        [Theory]
        [FixtureInlineAutoData("")]
        [FixtureInlineAutoData(null)]
        public void Should_NotBeValid_When_NameIsNullOrEmpty(string name)
        {
            // Arrange
            var command = new CreateTodoCommand
            {
                Name = name
            };

            // Act & Assert
            AssertNotValid(command);
        }

        [Fact]
        public void Should_NotBeValid_When_NameExceedsMaximumLength()
        {
            // Arrange
            var command = new CreateTodoCommand
            {
                Name = new string('A', 201) 
            };

            // Act & Assert
            AssertNotValid(command);
        }
    }
}
