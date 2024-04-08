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
        [Theory]
        [InlineData("test")]
        [InlineData("1")]
        [InlineData("12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890")]
        public void Should_BeValid_When_RequestIsValid(string name)
        {
            // arrange
            var create = new CreateTodoCommand
            {
                Name = name
            };

            // act & assert
            AssertValid(create);
        }
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901")]
        public void Should_BeNotValid_When_RequestNotValid(string? name)
        {
            // arrange
            var create = new CreateTodoCommand
            {
                Name = name
            };

            // act & assert
            AssertNotValid(create);
        }
    }
}
