using Core.Tests;
using System;
using AutoFixture;
using Core.Tests.Attributes;
using FluentValidation;
using Todos.Applications.Handlers.Queries.GetTodos;
using Xunit.Abstractions;
using Todos.Applications.Handlers.Commands.UpdateTodo;
using System.Text;

namespace Todos.UnitTests.Tests.Commands.UpdateTodo
{
    public class UpdateTodoCommandValidatorTest : ValidatorTestBase<UpdateTodoCommand>
    {
        public UpdateTodoCommandValidatorTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override IValidator<UpdateTodoCommand> TestValidator => TestFixture.Create<UpdateTodoCommandValidator>();

        [Fact]
        public void Should_BeValid_When_RequestIsValid()
        {
            // arrange
            var update = new UpdateTodoCommand
            {
                IsDone = true,
                Name = "Test",
                TodoId = 1,
            };
            // act & assert
            AssertValid(update);
        }
        [Fact]
        public void Should_NotValid_With_ZeroId()
        {
            // arrange
            var update = new UpdateTodoCommand
            {
                IsDone = true,
                Name = "1234",
                TodoId = 0
            };
            // act & assert
            AssertNotValid(update);
        }
        [Fact]
        public void Should_Valid_With_Valid_OneCharTodoName()
        {
            // arrange
            var update = new UpdateTodoCommand
            {
                IsDone = true,
                Name = "1",
                TodoId = 1
            };
            // act & assert
            AssertValid(update);
        }
        [Fact]
        public void Should_Valid_With_Valid_200CharTodoName()
        {
            // arrange
            var update = new UpdateTodoCommand
            {
                IsDone = true,
                Name = new string('1', 200),
                TodoId = 1
            };
            // act & assert
            AssertValid(update);
        }
        [Fact]
        public void Should_NotValid_With_NotValid_201CharTodoName()
        {
            // arrange
            var update = new UpdateTodoCommand
            {
                IsDone = true,
                Name = new string('1', 201),
                TodoId = 1
            };
            // act & assert
            AssertNotValid(update);
        }
    }
}
