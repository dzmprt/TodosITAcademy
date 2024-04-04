using AutoFixture;
using Core.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todos.Applications.Handlers.Commands.CreateTodo;
using Todos.Applications.Handlers.Commands.UpdateTodo;
using Xunit.Abstractions;

namespace Todos.UnitTests.Tests.Commands.CreateTodo
{
    public class CreateTodoCommandValidatorTest : ValidatorTestBase<CreateTodoCommand>
    {
        public CreateTodoCommandValidatorTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override FluentValidation.IValidator<CreateTodoCommand> TestValidator => TestFixture.Create<CreateTodoCommandValidator>();
        [Fact]
        public void Should_BeValid_When_RequestIsValid()
        {
            // arrange
            var create = new CreateTodoCommand
            {
                Name = "Test"
            };
            // act & assert
            AssertValid(create);
        }
        [Fact]
        public void Should_Valid_With_Valid_OneChatTodoName()
        {
            // arrange
            var create = new CreateTodoCommand
            {
                Name = "1",
            };
            // act & assert
            AssertValid(create);
        }
        [Fact]
        public void Should_Valid_With_Valid_200ChatTodoName()
        {
            // arrange
            var create = new CreateTodoCommand
            {
                Name = new string('1', 200)
            };
            // act & assert
            AssertValid(create);
        }
        [Fact]
        public void Should_NotValid_With_NotValid_201ChatTodoName()
        {
            // arrange
            var create = new CreateTodoCommand
            {
                Name = new string('1', 201)
            };
            // act & assert
            AssertNotValid(create);
        }
    }
}
